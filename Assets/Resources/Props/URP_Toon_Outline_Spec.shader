Shader "Toon/URP_Toon_Outline_Spec"
{
    Properties
    {
        _BaseMap("Base Map", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _Steps("Diffuse Steps", Range(2,8)) = 4

        _SpecColor("Specular Color", Color) = (1,1,1,1)
        _Shininess("Specular Exponent", Range(1,256)) = 64
        _SpecSteps("Specular Steps", Range(1,6)) = 2
        _SpecIntensity("Specular Intensity", Range(0,2)) = 1

        _AmbientBoost("Ambient Boost", Range(0,1)) = 0.2
        _ShadowStrength("Shadow Strength", Range(0,1)) = 1

        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth("Outline Width (world units)", Range(0.0,0.03)) = 0.01
    }

    SubShader
    {
        Tags{ "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry" }

        HLSLINCLUDE
        #pragma target 3.0
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
        #pragma multi_compile _ _ADDITIONAL_LIGHTS
        #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
        #pragma multi_compile _ _SHADOWS_SOFT
        #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

        CBUFFER_START(UnityPerMaterial)
            float4 _BaseColor;
            float4 _SpecColor;
            float4 _OutlineColor;
            float  _Steps;
            float  _Shininess;
            float  _SpecSteps;
            float  _SpecIntensity;
            float  _AmbientBoost;
            float  _OutlineWidth;
            float  _ShadowStrength;
        CBUFFER_END

        TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap); float4 _BaseMap_ST;

        struct Attributes
        {
            float4 positionOS : POSITION;
            float3 normalOS   : NORMAL;
            float2 uv         : TEXCOORD0;
        };

        struct Varyings
        {
            float4 positionHCS : SV_POSITION;
            float3 positionWS  : TEXCOORD0;
            float3 normalWS    : TEXCOORD1;
            float2 uv          : TEXCOORD2;
            float3 viewDirWS   : TEXCOORD3;
        };

        float MaxComponent(float3 v){ return max(v.x, max(v.y, v.z)); }

        // Compute world-space scale magnitude to keep outline constant in world units
        float GetMaxScale()
        {
            float3 x = float3(unity_ObjectToWorld._11, unity_ObjectToWorld._12, unity_ObjectToWorld._13);
            float3 y = float3(unity_ObjectToWorld._21, unity_ObjectToWorld._22, unity_ObjectToWorld._23);
            float3 z = float3(unity_ObjectToWorld._31, unity_ObjectToWorld._32, unity_ObjectToWorld._33);
            return max(length(x), max(length(y), length(z)));
        }

        Varyings VertToon(Attributes IN)
        {
            Varyings OUT;
            float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
            float3 normalWS   = TransformObjectToWorldNormal(IN.normalOS);

            OUT.positionHCS = TransformWorldToHClip(positionWS);
            OUT.positionWS = positionWS;
            OUT.normalWS = normalize(normalWS);
            OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);

            float3 camPosWS = GetCameraPositionWS();
            OUT.viewDirWS = normalize(camPosWS - positionWS);
            return OUT;
        }

        // Quantize a value into N steps in [0,1]
        float Quantize(float v, float steps)
        {
            steps = max(1.0, steps);
            return floor(saturate(v) * steps) / steps;
        }

        // Main toon+spec lighting
        float3 ToonLighting(float3 baseColor, float3 normalWS, float3 viewDirWS, float3 positionWS)
        {
            // Ambient (SH) + pequeño boost para look cartoon
            float3 sh = SampleSH(normalWS);
            float3 ambient = sh * baseColor + _AmbientBoost * baseColor;

            // Luz principal con sombras
            Light mainLight = GetMainLight(TransformWorldToShadowCoord(positionWS));
            float NdotL = max(0.0, dot(normalWS, mainLight.direction));
            float shadow = lerp(1.0, mainLight.shadowAttenuation, _ShadowStrength);
            float lit = NdotL * shadow;

            // Difuso en pasos
            float diffuseStep = Quantize(lit, _Steps - 1); // -1 para reservar 0 como sombra fuerte
            float3 diffuse = diffuseStep * (baseColor * mainLight.color);

            // Especular toon (Blinn-Phong quantized)
            float3 H = SafeNormalize(mainLight.direction + viewDirWS);
            float NdotH = max(0.0, dot(normalWS, H));
            float spec = pow(NdotH, _Shininess) * shadow;
            float specStep = Quantize(spec, _SpecSteps);
            float3 specular = specStep * _SpecIntensity * _SpecColor.rgb * mainLight.color;

            // Luces adicionales
            #if defined(_ADDITIONAL_LIGHTS)
            uint count = GetAdditionalLightsCount();
            for (uint i = 0; i < count; i++)
            {
                Light light = GetAdditionalLight(i, positionWS);
                float ndl = max(0.0, dot(normalWS, light.direction));
                float lLit = ndl; // sin sombras por simplicidad (URP las soporta si están activadas)
                float lStep = Quantize(lLit, _Steps - 1);
                diffuse += lStep * (baseColor * light.color);

                float3 H2 = SafeNormalize(light.direction + viewDirWS);
                float nDh = max(0.0, dot(normalWS, H2));
                float s2 = pow(nDh, _Shininess);
                float s2step = Quantize(s2, _SpecSteps);
                specular += s2step * _SpecIntensity * _SpecColor.rgb * light.color;
            }
            #endif

            return ambient + diffuse + specular;
        }
        ENDHLSL

        // -------- PASS 1: OUTLINE (backfaces extruded) --------
        Pass
        {
            Name "Outline"
            Tags { "LightMode"="SRPDefaultUnlit" }
            Cull Front     // renderizar caras traseras para que "asomen" como borde
            ZWrite On
            ZTest LEqual
            Blend Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct VOut
            {
                float4 pos : SV_POSITION;
            };

            VOut vert(Attributes IN)
            {
                VOut OUT;
                float maxScale = GetMaxScale();
                float3 nOS = normalize(IN.normalOS);
                // Extrusión en espacio de objeto, compensando el escalado para que el grosor sea en unidades de mundo
                float3 offsetOS = nOS * (_OutlineWidth / max(1e-5, maxScale));
                float3 posOS = IN.positionOS.xyz + offsetOS;
                float3 posWS = TransformObjectToWorld(posOS);
                OUT.pos = TransformWorldToHClip(posWS);
                return OUT;
            }

            half4 frag() : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }

        // -------- PASS 2: TOON LIGHTING --------
        Pass
        {
            Name "ForwardToon"
            Tags { "LightMode"="UniversalForward" }
            Cull Back
            ZWrite On
            ZTest LEqual
            Blend Off

            HLSLPROGRAM
            #pragma vertex VertToon
            #pragma fragment FragToon

            half4 FragToon(Varyings IN) : SV_Target
            {
                float4 albedoSample = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                float3 baseColor = albedoSample.rgb * _BaseColor.rgb;

                float3 n = normalize(IN.normalWS);
                float3 v = normalize(IN.viewDirWS);

                float3 litColor = ToonLighting(baseColor, n, v, IN.positionWS);

                // Respetar alfa del color base para posibles usos futuros (recortes, etc.)
                return half4(saturate(litColor), _BaseColor.a * albedoSample.a);
            }
            ENDHLSL
        }
    }

    FallBack Off
}
