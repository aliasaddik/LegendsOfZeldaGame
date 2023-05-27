// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "EGA/Particles/BlendDissolve"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "white" {}
		_Noise("Noise", 2D) = "white" {}
		_SpeedMainTexUVNoiseZW("Speed MainTex U/V + Noise Z/W", Vector) = (0,0,0,0)
		_Color("Color", Color) = (0.5,0.5,0.5,1)
		_Emission("Emission", Float) = 2
		_Textureglow("Texture glow", Range( 0 , 1)) = 0.4
		[MaterialToggle] _Usedepth ("Use depth?", Float ) = 0
        _Depthpower ("Depth power", Float ) = 1
		[Toggle]_UseTextureOpacity("Use Texture Opacity", Float) = 0
		[Toggle]_UseTextureEmission("Use Texture Emission", Float) = 0
		[Toggle]_Useremap("Use remap", Float) = 1
	}


	Category 
	{
		SubShader
		{
			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask RGB
			Cull Off
			Lighting Off 
			ZWrite Off
			ZTest LEqual
			
			Pass {
			
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile_particles
				#pragma multi_compile_fog
				#include "UnityShaderVariables.cginc"


				#include "UnityCG.cginc"

				struct appdata_t 
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
					
				};

				struct v2f 
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					#ifdef SOFTPARTICLES_ON
					float4 projPos : TEXCOORD2;
					#endif
					UNITY_VERTEX_INPUT_INSTANCE_ID
					UNITY_VERTEX_OUTPUT_STEREO
					
				};
				
				
				#if UNITY_VERSION >= 560
				UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
				#else
				uniform sampler2D_float _CameraDepthTexture;
				#endif

				//Don't delete this comment
				// uniform sampler2D_float _CameraDepthTexture;

				uniform sampler2D _MainTex;
				uniform fixed4 _TintColor;
				uniform float4 _MainTex_ST;
				uniform float _Depthpower;
				uniform float _Emission;
				uniform float4 _Color;
				uniform float _UseTextureEmission;
				uniform float4 _SpeedMainTexUVNoiseZW;
				uniform float _UseTextureOpacity;
				uniform float _Textureglow;
				uniform sampler2D _Noise;
				uniform float4 _Noise_ST;
				uniform float _Useremap;
				uniform fixed _Usedepth;

				v2f vert ( appdata_t v  )
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					UNITY_TRANSFER_INSTANCE_ID(v, o);
					

					v.vertex.xyz +=  float3( 0, 0, 0 ) ;
					o.vertex = UnityObjectToClipPos(v.vertex);
					#ifdef SOFTPARTICLES_ON
						o.projPos = ComputeScreenPos (o.vertex);
						COMPUTE_EYEDEPTH(o.projPos.z);
					#endif
					o.color = v.color;
					o.texcoord = v.texcoord;
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag ( v2f i  ) : SV_Target
				{
					float lp = 1;
					#ifdef SOFTPARTICLES_ON
						float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
						float partZ = i.projPos.z;
						float fade = saturate ((sceneZ-partZ) / _Depthpower);
						lp *= lerp(1, fade, _Usedepth);
						i.color.a *= lp;
					#endif

					float2 uv0_MainTex = i.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					float2 appendResult21 = (float2(_SpeedMainTexUVNoiseZW.x , _SpeedMainTexUVNoiseZW.y));
					float4 tex2DNode13 = tex2D( _MainTex, ( uv0_MainTex + ( appendResult21 * _Time.y ) ) );
					float4 uv0_Noise = i.texcoord;
					uv0_Noise.xy = i.texcoord.xy * _Noise_ST.xy + _Noise_ST.zw;
					float2 appendResult22 = (float2(_SpeedMainTexUVNoiseZW.z , _SpeedMainTexUVNoiseZW.w));
					float temp_output_70_0 = ( tex2DNode13.a * tex2D( _Noise, ( (uv0_Noise).xy + ( _Time.y * appendResult22 ) + uv0_Noise.w ) ).a );
					float temp_output_132_0 = (-0.65 + (uv0_Noise.z - 0.0) * (0.65 - -0.65) / (2.0 - 0.0));
					float clampResult148 = clamp( (-0.2 + (( ( _Textureglow * temp_output_70_0 * uv0_Noise.z ) + temp_output_132_0 ) - 0.0) * (1.0 - -0.2) / (1.0 - 0.0)) , 0.0 , 1.0 );
					float temp_output_133_0 = ( temp_output_70_0 + temp_output_132_0 );
					float clampResult136 = clamp( lerp(temp_output_133_0,(-12.0 + (temp_output_133_0 - 0.0) * (12.0 - -12.0) / (1.0 - 0.0)),_Useremap) , 0.0 , 1.0 );
					float clampResult143 = clamp( ( clampResult148 + clampResult136 ) , 0.0 , 1.0 );
					float4 appendResult156 = (float4((( _Emission * _Color * i.color * lerp(float4( 1,1,1,1 ),tex2DNode13,_UseTextureEmission) )).rgb , ( _Color.a * i.color.a * lerp(clampResult143,temp_output_70_0,_UseTextureOpacity) )));
					

					fixed4 col = appendResult156;
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG 
			}
		}	
	}
	
	
	
}
/*ASEBEGIN
Version=16700
7;29;1906;1004;1721.171;-107.3664;1.340763;True;False
Node;AmplifyShaderEditor.Vector4Node;15;-1663.553,623.6537;Float;False;Property;_SpeedMainTexUVNoiseZW;Speed MainTex U/V + Noise Z/W;2;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;22;-1299.607,783.8657;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;29;-1089.143,654.7549;Float;False;0;14;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;21;-1302.148,561.586;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TimeNode;17;-1332.031,652.2165;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;106;-859.0841,459.1281;Float;False;0;13;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;59;-810.5602,652.0452;Float;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-1083.032,829.7051;Float;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-1085.475,562.9271;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;27;-511.6236,753.8995;Float;False;3;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;26;-559.6987,540.5644;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;14;-364.3325,718.0491;Float;True;Property;_Noise;Noise;1;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;13;-363.9455,532.6113;Float;True;Property;_MainTex;Main Tex;0;0;Create;True;0;0;False;0;f51d122ef849e084dbac39d8c61a5785;f51d122ef849e084dbac39d8c61a5785;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;164;-662.6815,940.8672;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;141;-337.944,437.6385;Float;False;Property;_Textureglow;Texture glow;5;0;Create;True;0;0;False;0;0.4;0.2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;132;-268.4642,932.3643;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;2;False;3;FLOAT;-0.65;False;4;FLOAT;0.65;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;70;-36.83327,661.2621;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;133;268.3871,717.4963;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;140;131.7223,481.7012;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;146;300.4876,498.9465;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;134;407.5648,799.6103;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-12;False;4;FLOAT;12;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;165;659.539,683.0473;Float;False;Property;_Useremap;Use remap;8;0;Create;True;0;0;False;0;1;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;147;692.2294,503.1082;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-0.2;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;136;891.3667,669.7173;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;148;896.9679,505.591;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;151;151.2796,371.0267;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;142;1074.554,573.3091;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;150;891.5272,286.3305;Float;False;Property;_UseTextureEmission;Use Texture Emission;7;0;Create;True;0;0;False;0;0;2;0;COLOR;1,1,1,1;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;145;972.3101,-80.25921;Float;False;Property;_Color;Color;3;0;Create;True;0;0;False;0;0.5,0.5,0.5,1;0.5,0.5,0.5,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;154;429.3543,996.7526;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;52;1041.208,-191.4583;Float;False;Property;_Emission;Emission;4;0;Create;True;0;0;False;0;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;143;1236.103,592.1222;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;32;1008.806,98.2374;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;1442.244,54.06595;Float;False;4;4;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;153;1453.263,637.4864;Float;False;Property;_UseTextureOpacity;Use Texture Opacity;6;0;Create;True;0;0;False;0;0;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;144;1791.522,598.5775;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;157;1712.385,245.8021;Float;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;156;2017.776,395.85;Float;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;155;2278.745,406.2372;Float;False;True;2;Float;;0;11;EGA/Particles/BlendDissolve;0b6a9f8b4f707c74ca64c0be8e590de0;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;True;2;False;-1;True;True;True;True;False;0;False;-1;False;True;2;False;-1;True;3;False;-1;False;True;4;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;False;0;False;False;False;False;False;False;False;False;False;False;True;0;0;;0;0;Standard;0;0;1;True;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;22;0;15;3
WireConnection;22;1;15;4
WireConnection;21;0;15;1
WireConnection;21;1;15;2
WireConnection;59;0;29;0
WireConnection;23;0;17;2
WireConnection;23;1;22;0
WireConnection;24;0;21;0
WireConnection;24;1;17;2
WireConnection;27;0;59;0
WireConnection;27;1;23;0
WireConnection;27;2;29;4
WireConnection;26;0;106;0
WireConnection;26;1;24;0
WireConnection;14;1;27;0
WireConnection;13;1;26;0
WireConnection;164;0;29;3
WireConnection;132;0;164;0
WireConnection;70;0;13;4
WireConnection;70;1;14;4
WireConnection;133;0;70;0
WireConnection;133;1;132;0
WireConnection;140;0;141;0
WireConnection;140;1;70;0
WireConnection;140;2;29;3
WireConnection;146;0;140;0
WireConnection;146;1;132;0
WireConnection;134;0;133;0
WireConnection;165;0;133;0
WireConnection;165;1;134;0
WireConnection;147;0;146;0
WireConnection;136;0;165;0
WireConnection;148;0;147;0
WireConnection;151;0;13;0
WireConnection;142;0;148;0
WireConnection;142;1;136;0
WireConnection;150;1;151;0
WireConnection;154;0;70;0
WireConnection;143;0;142;0
WireConnection;51;0;52;0
WireConnection;51;1;145;0
WireConnection;51;2;32;0
WireConnection;51;3;150;0
WireConnection;153;0;143;0
WireConnection;153;1;154;0
WireConnection;144;0;145;4
WireConnection;144;1;32;4
WireConnection;144;2;153;0
WireConnection;157;0;51;0
WireConnection;156;0;157;0
WireConnection;156;3;144;0
WireConnection;155;0;156;0
ASEEND*/
//CHKSM=BDC813448DBE26D2E9B9BCE91209AC43010E31A5