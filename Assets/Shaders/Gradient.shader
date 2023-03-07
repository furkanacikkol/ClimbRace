Shader "Hifive Games/Toon_Gradient_3Color"
{
    Properties
    {
        _SecondaryColor("Secondary Color", Color) = (1,1,1,1)
        _Color("Main Color", Color) = (1,1,1,1)
        _ToonShade ("ToonShader Cubemap(RGB)", CUBE) = "" { }

        _Value("Value", Range(-0.2, 1.5)) = 1
        _Middle("Middle", Range(-0.999, 0.999)) = 1
        //_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
        _MainTex ("Base (RGB)", 2D) = "white" {}

    }

    SubShader
    {
        // Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
        Tags
        {
            "RenderType"="Opaque"
        }

        LOD 100

        ZWrite on

        Pass
        {
            Name "BASE"
            Cull Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float3 normal : NORMAL;

            };

            struct v2f
            {
                float4 pos : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				float3 cubenormal : TEXCOORD1;
				UNITY_FOG_COORDS(2)
                fixed4 col : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _SecondaryColor;
            fixed4 _Color;
            float _Middle;
            float _Value;
            samplerCUBE _ToonShade;


            v2f vert(appdata_t v)
            {
                v2f o;
                o.col = lerp(_Color, _SecondaryColor, (v.vertex.y + _Value) / _Middle) * step(v.vertex.y, _Middle);
                o.col += lerp(_SecondaryColor, _Color, ((v.vertex.y - _Value) - _Middle) / (_Middle)) * step(
                    _Middle, v.vertex.y);
                o.pos = UnityObjectToClipPos(v.vertex);
            	o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.cubenormal = mul (UNITY_MATRIX_MV, float4(v.normal,0));
				UNITY_TRANSFER_FOG(o,o.pos);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord) * i.col;
                fixed4 cube = texCUBE(_ToonShade, i.cubenormal);
                fixed4 c = fixed4(2.0f * cube.rgb * col.rgb, col.a);
                UNITY_APPLY_FOG(i.fogCoord, c);
                return c;
            }
            ENDCG
        }
    }
	Fallback "VertexLit"

}