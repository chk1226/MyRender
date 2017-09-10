@vertex shader
#version 330 
layout (location = 0) in vec3 position;
layout (location = 1) in float scale;
layout (location = 2) in float rotation;
layout (location = 3) in float blendFactor;
layout (location = 4) in vec4 texCoord;


uniform mat4 MODEL;
uniform mat4 VIEW;
uniform mat4 PROJECTION;

out VS_OUT {
    float scale;
	float rotation;
	float blendFactor;
	vec4 texCoord;
} vs_out;

void main(void)
{
	vec4 pos = vec4(position, 1);
	pos = PROJECTION * VIEW * MODEL * pos;
	
	vs_out.scale = scale;
	vs_out.rotation = rotation;
	vs_out.blendFactor = blendFactor;
	vs_out.texCoord = texCoord;

	gl_Position = pos;
}

@geometry shader
#version 330
layout (points) in;
layout (triangle_strip, max_vertices = 4) out;

uniform mat4 BoardMatrix;
uniform mat4 VIEW;
uniform mat4 PROJECTION;
uniform vec2 Offset;

in VS_OUT {
    float scale;
	float rotation;
	float blendFactor;
	vec4 texCoord;
} gs_in[];

out GS_OUT {
    vec2 texCurrent;
	vec2 texNext;
	float blendFactor;
} gs_out;

mat4 getRotationZ(float r)
{
	mat4 m;
	m[0] = vec4(cos(r), sin(r), 0, 0);
	m[1] = vec4(-sin(r), cos(r), 0, 0);
	m[2] = vec4(0, 0, 1, 0);
	m[3] = vec4(0, 0, 0, 1);

	return m;
}

void genSquare(vec4 position, float scale, float rotation, vec2 current, vec2 next, float blend)
{    
	vec4[4] offset = { vec4(-scale, -scale, 0, 0),
						vec4(scale, -scale, 0, 0),
						vec4(-scale, scale, 0, 0),
						vec4(scale, scale, 0, 0)};

	vec2[4] stOffset = { vec2(0, -Offset.y),
						vec2(Offset.x, -Offset.y),
						vec2(0, 0),
						vec2(Offset.x, 0)};

	mat4 m = PROJECTION * VIEW * BoardMatrix * getRotationZ(rotation);

    gl_Position = position + m * offset[0];    // 1:bottom-left
	gs_out.texCurrent = current + stOffset[0];
	gs_out.texNext = next + stOffset[0];
	gs_out.blendFactor = blend;
    EmitVertex();   

    gl_Position = position + m * offset[1];    // 2:bottom-right
	gs_out.texCurrent = current + stOffset[1];
	gs_out.texNext = next + stOffset[1];
	gs_out.blendFactor = blend;
    EmitVertex();

    gl_Position = position + m * offset[2];    // 3:top-left
	gs_out.texCurrent = current + stOffset[2];
	gs_out.texNext = next + stOffset[2];
	gs_out.blendFactor = blend;
    EmitVertex();

    gl_Position = position + m * offset[3];    // 4:top-right
	gs_out.texCurrent = current + stOffset[3];
	gs_out.texNext = next + stOffset[3];
	gs_out.blendFactor = blend;
    EmitVertex();


    EndPrimitive();
}

void main() {    
	genSquare(gl_in[0].gl_Position, gs_in[0].scale, gs_in[0].rotation, gs_in[0].texCoord.xy, gs_in[0].texCoord.zw, gs_in[0].blendFactor);
}  

@fragment shader
#version 330 
out vec4 FragColor;
uniform sampler2D TEX_COLOR;

in GS_OUT {
    vec2 texCurrent;
	vec2 texNext;
	float blendFactor;
} fs_in;

void main(void)
{
	vec4 color = mix(texture(TEX_COLOR, fs_in.texCurrent), texture(TEX_COLOR, fs_in.texNext), fs_in.blendFactor);

	FragColor = color;
}
