@vertex shader
void main(void)
{
	gl_Position = ftransform();
	gl_TexCoord[0] = gl_MultiTexCoord0;

}

@fragment shader
uniform vec2 Offset;
uniform sampler2D TEX_COLOR;

uniform float weight[5] = float[] (0.227027, 0.1945946, 0.1216216, 0.054054, 0.016216);
void main(void)
{	
	vec4 color = vec4(0.0);
	color += texture2D( TEX_COLOR, gl_TexCoord[0].st + vec2( -4.0*Offset.x, -4.0*Offset.y ) ) * weight[4];
	color += texture2D( TEX_COLOR, gl_TexCoord[0].st + vec2( -3.0*Offset.x, -3.0*Offset.y ) ) * weight[3];
	color += texture2D( TEX_COLOR, gl_TexCoord[0].st + vec2( -2.0*Offset.x, -2.0*Offset.y ) ) * weight[2];
	color += texture2D( TEX_COLOR, gl_TexCoord[0].st + vec2( -1.0*Offset.x, -1.0*Offset.y ) ) * weight[1];
	color += texture2D( TEX_COLOR, gl_TexCoord[0].st + vec2( 0.0 , 0.0) )*weight[0];
	color += texture2D( TEX_COLOR, gl_TexCoord[0].st + vec2( 1.0*Offset.x,  1.0*Offset.y ) ) * weight[1];
	color += texture2D( TEX_COLOR, gl_TexCoord[0].st + vec2( 2.0*Offset.x,  2.0*Offset.y ) ) * weight[2];
	color += texture2D( TEX_COLOR, gl_TexCoord[0].st + vec2( 3.0*Offset.x,  3.0*Offset.y ) ) * weight[3];
	color += texture2D( TEX_COLOR, gl_TexCoord[0].st + vec2( 4.0*Offset.x,  4.0*Offset.y ) ) * weight[4];


	gl_FragColor = color;	
}
