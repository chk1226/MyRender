@vertex shader
varying vec4 posE;	

void main(void)
{
	posE = gl_ModelViewMatrix * gl_Vertex;
	gl_TexCoord[0] = gl_MultiTexCoord0;

	gl_Position = ftransform();
}

@fragment shader
uniform sampler2D TEX_COLOR;
//uniform vec4 BASE_COLOR;

varying vec4 posE;	

void main(void)
{
	vec4 color = texture2D(TEX_COLOR, gl_TexCoord[0].st);	

	color.a = (color.r + color.g + color.b) / 3;

	//if(color.r < 0.3 && color.g < 0.3 && color.b < 0.3)
	//{
	//	color.a = 0;
	//}

	gl_FragColor = color;

	
}
