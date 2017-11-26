Shader "Projector/ProjectorShader" 
{
				Properties
				{
								_Color       ( "Tint Color", Color           ) = (1,1,1,1)
								_Attenuation ( "Falloff"   , Range(0.0, 1.0) ) = 1.0
								_ShadowTex   ( "Cookie"    , 2D              ) = "gray" {}
				}

				Subshader
				{
								Tags 
								{			
												"Queue" = "Transparent"
								}
								Pass 
								{
												ZWrite Off
												ColorMask RGB
												Blend SrcAlpha One // Additive blending
												Offset -1, -1

												CGPROGRAM
																// compilation directives for this snippet
																#pragma vertex vert
																#pragma fragment frag
																#include "UnityCG.cginc"

																// the Cg/HLSL code itself
																struct v2f 
																{
																				float4 uvShadow : TEXCOORD0;
																				float4 pos      : SV_POSITION;
																};

																float4x4 unity_Projector;
																float4x4 unity_ProjectorClip;

																v2f vert( float4 vertex : POSITION )
																{
																				v2f output;
																				output.pos      = UnityObjectToClipPos( vertex );
																				output.uvShadow = mul( unity_Projector, vertex );
																				return output;
																}

																sampler2D _ShadowTex;
																fixed4    _Color;
																float     _Attenuation;

																fixed4 frag( v2f input ) : SV_Target
																{
																				// Apply tint & alpha mask
																				fixed4 texCookie = tex2Dproj( _ShadowTex, UNITY_PROJ_COORD( input.uvShadow ) );
																				fixed4 outColor  = _Color * texCookie.a;
																				// Distance attenuation
																				float depth = input.uvShadow.z; // [-1(near), 1(far)]
																				return outColor * clamp(1.0 - abs(depth) + _Attenuation, 0.0, 1.0);
																}
												ENDCG
								}
				}
}