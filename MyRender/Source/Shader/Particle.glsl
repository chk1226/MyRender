@vertex shader
#version 330 
layout (location = 0) in vec3 position;
layout (location = 1) in float scale;
layout (location = 2) in float rotation;

uniform mat4 MODEL;
uniform mat4 VIEW;
uniform mat4 PROJECTION;

out VS_OUT {
    float scale;
	float rotation;
} vs_out;

void main(void)
{
	vec4 pos = vec4(position, 1);
	pos = PROJECTION * VIEW * MODEL * pos;
	
	vs_out.scale = scale;
	vs_out.rotation = rotation;

	gl_Position = pos;
}

@geometry shader
#version 330
layout (points) in;
layout (triangle_strip, max_vertices = 4) out;

uniform mat4 BoardMatrix;
uniform mat4 VIEW;
uniform mat4 PROJECTION;

in VS_OUT {
    float scale;
	float rotation;
} gs_in[];


mat4 getRotationZ(float r)
{
	mat4 m;
	m[0] = vec4(cos(r), sin(r), 0, 0);
	m[1] = vec4(-sin(r), cos(r), 0, 0);
	m[2] = vec4(0, 0, 1, 0);
	m[3] = vec4(0, 0, 0, 1);

	return m;
}

void genSquare(vec4 position, float scale, float rotation)
{    
	vec4[4] offset = { vec4(-scale, -scale, 0, 0),
						vec4(scale, -scale, 0, 0),
						vec4(-scale, scale, 0, 0),
						vec4(scale, scale, 0, 0)};

	mat4 m = PROJECTION * VIEW * BoardMatrix * getRotationZ(rotation);

    gl_Position = position + m * offset[0];    // 1:bottom-left
    EmitVertex();   
    gl_Position = position + m * offset[1];    // 2:bottom-right
    EmitVertex();
    gl_Position = position + m * offset[2];    // 3:top-left
    EmitVertex();
    gl_Position = position + m * offset[3];    // 4:top-right
    EmitVertex();
    EndPrimitive();
}

void main() {    
	genSquare(gl_in[0].gl_Position, gs_in[0].scale, gs_in[0].rotation);
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
