Shader "Unlit/019"
{
    Properties
    {
        _MainTex ("主贴图,メインスタンプ,Texture", 2D) = "white" {}
		_Diffuse ("漫反射,拡散反射,Color", color) = (1,1,1,1)
		//描边
		_Outline ("描边范围,引き取り範囲,Outline", Range(0,0.01)) = 0.008
		//描边颜色
		_OutlineColor ("描边颜色,エッジの色,OutlineColor", color) = (0,0,0,0) 
		//分层数（用于漫反射显示人物阴影）
		_Steps ("分层数,レイヤー数,Steps", Range(1,30))  = 1
		//分层数的效果，0是没有 1是很高
		_ToonEffect ("分层数的效果,レイヤー数の効果,ToonEffect", Range(0,1)) = 0.5
		//渐进纹理贴图，嘛就是跟分层一样 = =
		_RampTex ("渐进纹理贴图,漸進テクスチャスタンプ,RampTex", 2D) = "white" {}
		//边缘光颜色
		_RimColor ("边缘光颜色,エッジ光の色,RimColor", color) = (1,1,1,1) 
		//边缘光强度
		_RimPower ("边缘光强度,エッジ光強度,RimPower", Range(0.0001,10))  = 0.2
		//XRay颜色
		_XRayColor ("XRay颜色,XRay色,XRayColor", color) = (1,1,1,1) 
		//XRay强度
		_XRayPower ("XRay强度,XRay強度,XRayPower", Range(0.0001,10))  = 0.2

    }
    SubShader
    {
		//在不透明物体后面渲染Geometry+1000中间不能加空格
        Tags {"Queue" = "Geometry+1000" "RenderType"="Opaque" }
        LOD 100

		//XRay
		Pass
		{
			Name "XRay"
			Tags {"ForceNoShadowCasting" = "true" }	
			//透明
			Blend SrcAlpha One	
			//深度写入关
			ZWrite Off
			//深度测试改为被挡住时渲染
			ZTest Greater
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			float _XRayPower;
			fixed4 _XRayColor;

			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed3 normal : TEXCOORD0;				
				fixed3 viewDir : TEXCOORD1;	
			};
			
			v2f vert(appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = v.normal;
				o.viewDir = ObjSpaceViewDir(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
            {
				fixed3 normal = normalize(i.normal);
				fixed3 viewDir = normalize(i.viewDir);
				float rim = 1-dot(normal,viewDir);
				//return _XRayColor * pow(rim, 1/_XRayPower);
				return _XRayColor * rim * _XRayPower;
			}

			ENDCG
		}

		//描边
		Pass
		{
			//起名用于可以重复调用
			Name "OutLine"
			//裁剪掉前面,默认都是裁剪掉后面，减少资源消耗，因为是描边，所以裁剪掉前面。
			Cull Front
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			float _Outline;
			fixed4 _OutlineColor;


			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				//物体空间下的描边（法线外拓）
				v.vertex.xyz += v.normal * _Outline;
				o.vertex = UnityObjectToClipPos(v.vertex);

				//视角空间下的描边（法线外拓）
				//float4 pos = mul(UNITY_MATRIX_V, mul(unity_ObjectToWorld, v.vertex));
				//float3 normal = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, v.normal));
				//pos = pos + float4(normal,0) * _Outline;
				//o.vertex = mul(UNITY_MATRIX_P, pos);
				
				//裁剪空间下的描边（法线外拓）
				//o.vertex = UnityObjectToClipPos(v.vertex);
				//float3 normal = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, v.normal));
				//float2 viewNormal = TransformViewToProjection(normal.xy);
				//o.vertex.xy += viewNormal * _Outline;

				return o;
			}

			fixed4 frag(v2f i) :SV_Target
			{
				return _OutlineColor;
			}

			ENDCG
		}
		//主渲染
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
           

            #include "UnityCG.cginc"
			#include "Lighting.cginc"

            struct v2f
            {
				float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
				fixed3 worldNormal : TEXCOORD1;				
				fixed3 worldPos : TEXCOORD2;				
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			fixed4 _Diffuse;
			//分层
			float _Steps;
			float _ToonEffect;
			//因为用不到UV所以就不用_RampTex_ST
			sampler _RampTex;
			//边缘光
			fixed3 _RimColor;
			float _RimPower;
	

            v2f vert (appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);

                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
               
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT;               
				fixed4 albedo = tex2D(_MainTex, i.uv);
				//世界光方向
				fixed3 worldLightDir = UnityWorldSpaceLightDir(i.worldPos);
				fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));

				
				//卡通着色
				float difLight = dot(worldLightDir,i.worldNormal) *0.5 +0.5;

				//颜色平滑【0,1】之间,整体能变亮一点
				difLight = smoothstep(0,1,difLight);
				//颜色分层（离散化）  先乘再除能够得到更多分层  toon块
				float toon = floor(difLight * _Steps) / _Steps;
				//插值平滑分层
				difLight = lerp(difLight,toon, _ToonEffect);
				//渐进贴图代替离散化
				//fixed4 rampColor  = tex2D(_RampTex,fixed2(difLight, difLight));
				//边缘光
				float rim = 1- dot(i.worldNormal, worldViewDir);
				fixed3 rimColor = _RimColor * pow(rim, 1/_RimPower);		

				fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb  * albedo * difLight; //rampColor;

                return fixed4(diffuse + ambient + rimColor,1);
            }
            ENDCG
        }
    }
	FallBack "Diffuse"

}
