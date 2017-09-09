@vertex shader
#version 330 
layout (location = 0) in vec3 position;

uniform mat4 MODELVIEW;
uniform mat4 PROJECTION;

void main(void)
{
//	posE = gl_ModelViewMatrix * gl_Vertex;
//	gl_TexCoord[0] = gl_MultiTexCoord0;
	vec4 pos = vec4(position, 1);
	pos = PROJECTION * MODELVIEW * pos;

	gl_Position = pos;
}

@geometry shader
#version 330
layout (points) in;
layout (triangle_strip, max_vertices = 4) out;

void genSquare(vec4 position)
{    
    gl_Position = position + vec4(-1, -1, 0.0, 0.0);    // 1:bottom-left
    EmitVertex();   
    gl_Position = position + vec4( 1, -1, 0.0, 0.0);    // 2:bottom-right
    EmitVertex();
    gl_Position = position + vec4(-1,  1, 0.0, 0.0);    // 3:top-left
    EmitVertex();
    gl_Position = position + vec4( 1,  1, 0.0, 0.0);    // 4:top-right
    EmitVertex();
    EndPrimitive();
}

void main() {    
	genSquare(gl_in[0].gl_Position);
}  

@fragment shader
#version 330 
out vec4 FragColor;
//uniform sampler2D TEX_COLOR;
//uniform vec4 BASE_COLOR;

//varying vec4 posE;	

void main(void)
{
	vec4 color = vec4(1,0,0,1);	

	FragColor = color;
}
