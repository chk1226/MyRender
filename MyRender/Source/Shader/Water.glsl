@vertex shader
varying vec4 posP;
varying vec4 reflectionPosP;

uniform mat4 REFLECTION_PVM;


void main(void)
{
    posP = gl_ModelViewProjectionMatrix * gl_Vertex;
	gl_TexCoord[0] = gl_MultiTexCoord0;
	reflectionPosP = REFLECTION_PVM * gl_Vertex;
	gl_Position = ftransform();

}

@fragment shader
uniform sampler2D REFLECTION;
uniform sampler2D REFRACTION;
uniform sampler2D DUDVMAP;
uniform float MoveFactor;

varying vec4 posP;
varying vec4 reflectionPosP;

const float waveStrength = 0.01;

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

    vec4 posProject = posP/posP.w;
    posProject = posProject/2 + 0.5;

	vec4 rPosP = reflectionPosP/reflectionPosP.w;
	rPosP = rPosP/2 + 0.5;

    vec2 ref = vec2(rPosP.x, rPosP.y);
	vec4 reflection = texture2D(REFLECTION, ref + total);	
	vec4 refraction = texture2D(REFRACTION, posProject.xy + total);	

	vec4 color = mix(reflection, refraction, 0.5);

	// parallel light
	//gl_FragColor = BlinnPhong(color, DIR_LIGHT, normalize(normalE), normalize(-posE.xyz));
	
    gl_FragColor = mix(color, vec4(0, 0.3, 0.5, 1), 0.1);
}
