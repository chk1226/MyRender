@vertex shader
varying vec3 TexCoord;	


void main(void)
{
	vec4 pos = gl_ModelViewProjectionMatrix * gl_Vertex;
	TexCoord = gl_Vertex.xyz;
	gl_Position = pos.xyww;
}

@fragment shader
varying vec3 TexCoord;	

uniform samplerCube cubemapTexture;

void main(void)
{

	gl_FragColor = textureCube(cubemapTexture, TexCoord);
	
}