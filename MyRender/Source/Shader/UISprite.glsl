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
uniform vec4 BASE_COLOR;

varying vec4 posE;	

void main(void)
{
	vec4 color = texture2D(TEX_COLOR, gl_TexCoord[0].st);	

	gl_FragColor = BASE_COLOR + color;

	
}
