@vertex shader
varying vec4 posE;	
varying vec3 normalE;	
varying vec3 t;

uniform mat4 ModelMatrix;
uniform vec4 ClipPlane;

attribute vec3 tangent;

void main(void)
{
	posE = gl_ModelViewMatrix * gl_Vertex;
    normalE = gl_NormalMatrix * gl_Normal;
	gl_TexCoord[0] = gl_MultiTexCoord0;

	t = normalize(tangent);
	t = normalize(gl_NormalMatrix * t);
	t = normalize(t-dot(normalE,t)*normalE);	

	gl_Position = ftransform();

	gl_ClipDistance[0] = dot(ModelMatrix * gl_Vertex, ClipPlane);
}

@fragment shader
uniform sampler2D TEX_COLOR;
uniform sampler2D NORMAL_TEX_COLOR;
uniform vec3 DIR_LIGHT;

varying vec4 posE;	
varying vec3 normalE;
varying vec3 t;

//parallel light
vec4 BlinnPhong(vec4 orign_color, vec3 dir_l, vec3 normal, vec3 v)
{
	//ambient
	vec4 La = gl_LightSource[0].ambient;
		
	//diffuse <N,L>*I
	float dot_value = max( dot( normal, dir_l), 0.0 );
	vec4 Ld = dot_value * gl_LightSource[0].diffuse;
				
	//specular <N,H>^n * I
	vec3 H = normalize( v + dir_l );
	dot_value = pow( max(dot(H, normal ), 0.0), 64);	
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

	gl_FragColor = BlinnPhong(color, normalize(dirL_tbn), normal_map, normalize(eye_tbn));

}
