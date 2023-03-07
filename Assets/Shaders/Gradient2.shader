Shader "Hifive Games/Gradient_3Color" {
    Properties{
        _SecondaryColor("Secondary Color", Color) = (1,1,1,1)
        _MainColor("Main Color", Color) = (1,1,1,1)
        
        _Value("Value", Range(-0.2, 1.5)) = 1
          _Middle("Middle", Range(-0.999, 0.999)) = 1
        _MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
    }

        SubShader{
            Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
            LOD 100

            ZWrite on

            Pass {
                CGPROGRAM
                    #pragma vertex vert
                    #pragma fragment frag

                    #include "UnityCG.cginc"

                    struct appdata_t {
                        float4 vertex : POSITION;
                        float2 texcoord : TEXCOORD0;
                    };

                    struct v2f {
                        float4 vertex : SV_POSITION;
                        half2 texcoord : TEXCOORD0;
                        fixed4 col : COLOR;
                    };

                    sampler2D _MainTex;
                    float4 _MainTex_ST;
                    fixed4 _SecondaryColor;
                    fixed4 _MainColor;
                    float  _Middle;
                    float  _Value;

                    v2f vert(appdata_t v)
                    {
                        v2f o;
                        o.col = lerp(_MainColor, _SecondaryColor, (v.vertex.y + _Value) / _Middle) * step(v.vertex.y, _Middle);
                        o.col += lerp(_SecondaryColor, _MainColor,((v.vertex.y - _Value) - _Middle) / ( _Middle)) * step(_Middle, v.vertex.y);
                        o.vertex = UnityObjectToClipPos(v.vertex);
                        o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                        return o;
                    }

                    fixed4 frag(v2f i) : SV_Target
                    {
                        fixed4 col = tex2D(_MainTex, i.texcoord) * i.col;
                        return col;
                    }
                ENDCG
            }
    }

}