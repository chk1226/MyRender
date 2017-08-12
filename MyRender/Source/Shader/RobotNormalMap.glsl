@vertex shader
varying vec4 posE;	
varying vec3 normalE;	
varying vec3 t;

attribute vec3 tangent;
attribute vec4 jointIndices;
attribute vec4 weight;

uniform mat4 jointTransforms[92];


void main(void)
{
	vec4 totalLocalPos = vec4(0.0);
	vec4 totalNormal = vec4(0.0);

	mat4 jt;
	vec4 posePosition;
	vec4 worldNormal;
	for(int i = 0; i < 4; i++)
	{
		jt = jointTransforms[int(jointIndices[i])];
		posePosition = jt * gl_Vertex;
		totalLocalPos += posePosition * weight[i];
		
		worldNormal = jt * vec4(gl_Normal, 0);
		totalNormal += worldNormal * weight[i];
	}

	posE = gl_ModelViewMatrix * totalLocalPos;
    normalE = gl_NormalMatrix * totalNormal.xyz;
	gl_TexCoord[0] = gl_MultiTexCoord0;

	t = normalize(tangent);
	t = normalize(gl_NormalMatrix * t);
	t = normalize(t-dot(normalE,t)*normalE);	

	gl_Position = gl_ModelViewProjectionMatrix * totalLocalPos;

}

@fragment shader
uniform sampler2D TEX_COLOR;
uniform sampler2D NORMAL_TEX_COLOR;
uniform sampler2D TEX_GLOW;
uniform sampler2D TEX_SPECULAR;
uniform vec3 DIR_LIGHT;

varying vec4 posE;	
varying vec3 normalE;
varying vec3 t;

//parallel light
vec4 BlinnPhong(vec4 orign_color, vec3 dir_l, vec3 normal, vec3 v, float shininess)
{
	//ambient
	vec4 La = gl_LightSource[0].ambient;
		
	//diffuse <N,L>*I
	float dot_value = max( dot( normal, dir_l), 0.0 );
	vec4 Ld = dot_value * gl_LightSource[0].diffuse;
				
	//specular <N,H>^n * I
	vec3 H = normalize( v + dir_l );
	dot_value = pow( max(dot(H, normal ), 0.0), shininess);	
	vec4 Ls = gl_LightSource[0].specular * dot_value;
	
	return orign_color * min((La + Ld + Ls), 1.0);
}


void main(void)
{
	vec4 color = texture2D(TEX_COLOR, gl_TexCoord[0].st);
	vec3 normal_map = texture2D(NORMAL_TEX_COLOR, gl_TexCoord[0].st).xyz;
	//[0~1] -> [-1~1]	
	normal_map = normal_map * 2.0 - 1.0;

	// bitangent
	vec3 b = normalize(cross(normalE, t));

	// parallel light
	vec3 dirL_tbn;
	dirL_tbn.x = dot(DIR_LIGHT, t); 
    dirL_tbn.y = dot(DIR_LIGHT, b); 
    dirL_tbn.z = dot(DIR_LIGHT, normalE); 

	// eye_tbn
	vec3 eye_tbn;
	eye_tbn.x = dot(-posE.xyz, t); 
    eye_tbn.y = dot(-posE.xyz, b); 
    eye_tbn.z = dot(-posE.xyz, normalE); 

	float shininess = texture2D(TEX_SPECULAR, gl_TexCoord[0].st).x * 255;
	vec4 result = BlinnPhong(color, normalize(dirL_tbn), normal_map, normalize(eye_tbn), shininess);
	result.xyz += texture2D(TEX_GLOW, gl_TexCoord[0].st).xyz;


	gl_FragColor = result;

}
