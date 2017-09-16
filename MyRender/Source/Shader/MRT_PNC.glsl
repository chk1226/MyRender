@vertex shader
#version 120 

varying vec4 posE;
varying vec3 normalE;	

void main(void)
{
	posE = gl_ModelViewMatrix * gl_Vertex;
    normalE = gl_NormalMatrix * gl_Normal;
	
	gl_Position = ftransform();
}

@fragment shader
#version 120 

varying vec4 posE;	
varying vec3 normalE;	

uniform vec3 Color;


void main(void)
{				
	gl_FragData[0] = posE;
	gl_FragData[1] = vec4(normalize(normalE), 0);
	gl_FragData[2] = vec4(Color, 1);
}