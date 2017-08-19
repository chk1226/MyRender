@vertex shader
varying vec4 posP;
varying vec4 reflectionPosP;

uniform mat4 REFLECTION_PVM;


void main(void)
{
    posP = gl_ModelViewProjectionMatrix * gl_Vertex;
	reflectionPosP = REFLECTION_PVM * gl_Vertex;
	gl_Position = ftransform();
}

@fragment shader
uniform sampler2D REFLECTION;
uniform sampler2D REFRACTION;

varying vec4 posP;
varying vec4 reflectionPosP;


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
    vec4 posProject = posP/posP.w;
    posProject = posProject/2 + 0.5;

	vec4 rPosP = reflectionPosP/reflectionPosP.w;
	rPosP = rPosP/2 + 0.5;

    vec2 ref = vec2(rPosP.x, rPosP.y);
	vec4 reflection = texture2D(REFLECTION, ref);	
	vec4 refraction = texture2D(REFRACTION, posProject.xy);	

	vec4 color = mix(reflection, refraction, 0.5);

	// parallel light
	//gl_FragColor = BlinnPhong(color, DIR_LIGHT, normalize(normalE), normalize(-posE.xyz));
	
    gl_FragColor = color;
}
