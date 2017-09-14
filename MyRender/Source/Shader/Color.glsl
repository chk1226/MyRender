@vertex shader
#version 120 

void main(void)
{
	gl_TexCoord[0] = gl_MultiTexCoord0;

	gl_Position = ftransform();
}

@fragment shader
#version 120 

uniform vec3 Color;

void main(void)
{
	vec4 color = vec4(Color, 1);	

	gl_FragColor = color;

}
