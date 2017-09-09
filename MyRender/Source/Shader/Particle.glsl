@vertex shader
#version 120 
//varying vec4 posE;	

void main(void)
{
//	posE = gl_ModelViewMatrix * gl_Vertex;
//	gl_TexCoord[0] = gl_MultiTexCoord0;

	gl_Position = ftransform();
}

@fragment shader
#version 120 

//uniform sampler2D TEX_COLOR;
//uniform vec4 BASE_COLOR;

//varying vec4 posE;	

void main(void)
{
	vec4 color = vec4(1,0,0,1);	

	gl_FragColor = color;

	
}
