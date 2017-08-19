@vertex shader
varying vec4 posP;
varying vec4 reflectionPosP;
varying vec3 ToCamera;

uniform mat4 REFLECTION_PVM;
uniform mat4 ModelMatrix;
uniform vec3 CameraPos;

void main(void)
{
    posP = gl_ModelViewProjectionMatrix * gl_Vertex;
	gl_TexCoord[0] = gl_MultiTexCoord0;
	reflectionPosP = REFLECTION_PVM * gl_Vertex;
	ToCamera = CameraPos - (ModelMatrix * gl_Vertex).xyz;
	gl_Position = ftransform();

}

@fragment shader
uniform sampler2D REFLECTION;
uniform sampler2D REFRACTION;
uniform sampler2D DUDVMAP;
uniform float MoveFactor;

varying vec4 posP;
varying vec4 reflectionPosP;
varying vec3 ToCamera;

const float waveStrength = 0.005;

//parallel light
//vec4 BlinnPhong(vec4 orign_color, vec3 dir_l, vec3 normal, vec3 v)
//{
//	//ambient
//	vec4 La = gl_LightSource[0].ambient;
		
//	//diffuse <N,L>*I
//	float dot_value = max( dot( normal, dir_l), 0.0 );
//	vec4 Ld = dot_value * gl_LightSource[0].diffuse;
				
//	//specular <N,H>^n * I
//	vec3 H = normalize( v + dir_l );
//	dot_value = pow( max(dot(H, normal ), 0.0), 0);	
//	vec4 Ls = gl_LightSource[0].specular * dot_value;
	
//	return orign_color * min((La + Ld + Ls * 0.3), 1.0);
//}

void main(void)
{

	vec2 distor1 = (texture2D(DUDVMAP, gl_TexCoord[0].st + vec2(MoveFactor, 0)).rg * 2.0 - 1.0) * waveStrength;
	vec2 distor2 = (texture2D(DUDVMAP, gl_TexCoord[0].st + vec2(-MoveFactor, MoveFactor)).rg * 2.0 - 1.0) * waveStrength;
	vec2 total = distor1 + distor2;

    vec4 posProject = posP / posP.w;
    posProject = posProject / 2 + 0.5;

	vec4 rPosP = reflectionPosP / reflectionPosP.w;
	rPosP = rPosP / 2 + 0.5;

	vec4 reflection = texture2D(REFLECTION, rPosP.xy + total);	
	// clamp is prevent glitch
	vec4 refraction = texture2D(REFRACTION, clamp(posProject.xy + total, 0.001, 0.999));	

	// Fresnel Effect
	float refractionFactor = dot(normalize(ToCamera), vec3(0, 1, 0));
	// pow is tweak value
	vec4 color = mix(reflection, refraction, pow(refractionFactor, 2));

	// parallel light
	//gl_FragColor = BlinnPhong(color, DIR_LIGHT, normalize(normalE), normalize(-posE.xyz));
	
    gl_FragColor = mix(color, vec4(0, 0.3, 0.5, 1), 0.1);
}
