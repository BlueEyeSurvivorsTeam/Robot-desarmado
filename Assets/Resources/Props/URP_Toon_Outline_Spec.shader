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

        [Toggle] _UseEmission("Use Emission", Float) = 0
        _EmissionColor("Emission Color", Color) = (1,1,1,1)
        _EmissionIntensity("Emission Intensity", Range(0,5)) = 1

        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth("Outline Width (world units)", Range(0.0,0.03)) = 0.01
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
            "Queue"="Geometry"
        }

        HLSLINCLUDE

        #pragma target 3.0

        // URP keywords
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
        #pragma multi_compile _ _ADDITIONAL_LIGHTS
        #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
        #pragma multi_compile _ _SHADOWS_SOFT
        #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION

        // URP includes
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

            float  _UseEmission;
            float4 _EmissionColor;
            float  _EmissionIntensity;
        CBUFFER_END

        TEXTURE2D(_BaseMap);
        SAMPLER(sampler_BaseMap);
        float4 _BaseMap_ST;

        struct Attributes
        {
            float4 positionOS : POSITION;
            float3 normalOS   : NORMAL;
            float2 uv         : TEXCOORD0;
        };

        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float3 positionWS : TEXCOORD0;
            float3 normalWS   : TEXCOORD1;
            float2 uv         : TEXCOORD2;
            float3 viewDirWS  : TEXCOORD3;
        };

        // --- Utilidades ---

        // Para que el grosor del outline se mantenga más o menos constante en mundo
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

            OUT.positionCS    = TransformWorldToHClip(positionWS);
            OUT.positionWS    = positionWS;
            OUT.normalWS      = normalize(normalWS);
            OUT.uv            = TRANSFORM_TEX(IN.uv, _BaseMap);

            float3 camPosWS   = GetCameraPositionWS();
            OUT.viewDirWS     = camPosWS - positionWS;

            return OUT;
        }

        // Cuantiza un valor a N pasos en [0,1]
        float Quantize(float v, float steps)
        {
            steps = max(1.0, steps);
            return floor(saturate(v) * steps) / steps;
        }

        // Toon + spec básico con luces URP
        float3 ToonLighting(float3 baseColor, float3 normalWS, float3 viewDirWS, float3 positionWS)
        {
            normalWS  = normalize(normalWS);
            viewDirWS = normalize(viewDirWS);

            // Ambient simple configurable
            float3 ambient = _AmbientBoost * baseColor;

            // Luz principal URP + sombras
            float4 shadowCoord = TransformWorldToShadowCoord(positionWS);
            Light mainLight = GetMainLight(shadowCoord);

            float NdotL   = max(0.0, dot(normalWS, mainLight.direction));
            float shadow  = mainLight.shadowAttenuation;
            float lit     = NdotL * lerp(1.0, shadow, _ShadowStrength);

            float diffuseStep = Quantize(lit, _Steps - 1);
            float3 diffuse    = diffuseStep * (baseColor * mainLight.color);

            // Especular toon (Blinn-Phong)
            float3 H       = SafeNormalize(mainLight.direction + viewDirWS);
            float NdotH    = max(0.0, dot(normalWS, H));
            float spec     = pow(NdotH, _Shininess) * shadow;
            float specStep = Quantize(spec, _SpecSteps);
            float3 specular = specStep * _SpecIntensity * _SpecColor.rgb * mainLight.color;

            // Luces adicionales
            #if defined(_ADDITIONAL_LIGHTS)
            uint count = GetAdditionalLightsCount();
            for (uint i = 0; i < count; i++)
            {
                Light light = GetAdditionalLight(i, positionWS);
                float ndl   = max(0.0, dot(normalWS, light.direction));
                float lLit  = ndl;
                float lStep = Quantize(lLit, _Steps - 1);

                diffuse += lStep * (baseColor * light.color);

                float3 H2    = SafeNormalize(light.direction + viewDirWS);
                float nDh    = max(0.0, dot(normalWS, H2));
                float s2     = pow(nDh, _Shininess);
                float s2step = Quantize(s2, _SpecSteps);

                specular += s2step * _SpecIntensity * _SpecColor.rgb * light.color;
            }
            #endif

            return ambient + diffuse + specular;
        }

        ENDHLSL

        // =========================
        //   PASS 1: OUTLINE
        // =========================
        Pass
        {
            Name "Outline"
            Tags { "LightMode"="SRPDefaultUnlit" }

            Cull Front
            ZWrite On
            ZTest LEqual
            Blend Off

            HLSLPROGRAM
            #pragma vertex VertOutline
            #pragma fragment FragOutline

            struct OutlineAttributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct OutlineVaryings
            {
                float4 pos : SV_POSITION;
            };

            OutlineVaryings VertOutline(OutlineAttributes IN)
            {
                OutlineVaryings OUT;

                float3 nOS     = normalize(IN.normalOS);
                float  maxScale = GetMaxScale();
                float3 offsetOS = nOS * (_OutlineWidth / max(1e-5, maxScale));
                float3 posOS    = IN.positionOS.xyz + offsetOS;

                float3 posWS = TransformObjectToWorld(posOS);
                OUT.pos      = TransformWorldToHClip(posWS);
                return OUT;
            }

            half4 FragOutline() : SV_Target
            {
                return _OutlineColor;
            }

            ENDHLSL
        }

        // =========================
        //   PASS 2: TOON + EMISSION
        // =========================
        Pass
        {
            Name "ForwardToon"
            Tags { "LightMode"="UniversalForward" }

            Cull Back
            ZWrite On
            ZTest LEqual
            Blend Off

            HLSLPROGRAM
            #pragma vertex   VertToon
            #pragma fragment FragToon

            half4 FragToon(Varyings IN) : SV_Target
            {
                float4 albedoSample = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                float3 baseColor    = albedoSample.rgb * _BaseColor.rgb;

                float3 n = normalize(IN.normalWS);
                float3 v = normalize(IN.viewDirWS);

                float3 litColor = ToonLighting(baseColor, n, v, IN.positionWS);

                // Emisivo opcional
                if (_UseEmission > 0.5)
                {
                    float3 emission = _EmissionColor.rgb * _EmissionIntensity;
                    litColor += emission;
                }

                return half4(saturate(litColor), _BaseColor.a * albedoSample.a);
            }

            ENDHLSL
        }
    }

    FallBack Off
}
