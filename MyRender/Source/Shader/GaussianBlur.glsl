@vertex shader
void main(void)
{
	gl_Position = ftransform();
	gl_TexCoord[0] = gl_MultiTexCoord0;

}

@fragment shader
uniform vec2 Offset;
uniform sampler2D TEX_COLOR;

void main(void)
{	
	vec4 color = vec4(0.0);
	color += texture2D( TEX_COLOR, gl_TexCoord[0].st + vec2( -3.0*Offset.x, -3.0*Offset.y ) ) * 0.015625;
	color += texture2D( TEX_COLOR, gl_TexCoord[0].st + vec2( -2.0*Offset.x, -2.0*Offset.y ) ) * 0.09375;
	color += texture2D( TEX_COLOR, gl_TexCoord[0].st + vec2( -1.0*Offset.x, -1.0*Offset.y ) ) * 0.234375;
	color += texture2D( TEX_COLOR, gl_TexCoord[0].st + vec2( 0.0 , 0.0) )*0.3125;
	color += texture2D( TEX_COLOR, gl_TexCoord[0].st + vec2( 1.0*Offset.x,  1.0*Offset.y ) ) * 0.234375;
	color += texture2D( TEX_COLOR, gl_TexCoord[0].st + vec2( 2.0*Offset.x,  2.0*Offset.y ) ) * 0.09375;
	color += texture2D( TEX_COLOR, gl_TexCoord[0].st + vec2( 3.0*Offset.x, -3.0*Offset.y ) ) * 0.015625;

	gl_FragColor = color;	
}
