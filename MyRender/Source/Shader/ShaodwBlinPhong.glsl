@vertex shader
varying vec4 posE;	
varying vec3 normalE;	
varying vec4 lightPosP;

uniform mat4 LIGHT_PVM;

void main(void)
{
	posE = gl_ModelViewMatrix * gl_Vertex;
    normalE = gl_NormalMatrix * gl_Normal;
	gl_TexCoord[0] = gl_MultiTexCoord0;
	lightPosP = LIGHT_PVM * gl_Vertex;

	gl_Position = ftransform();
}

@fragment shader
uniform sampler2D TEX_COLOR;
uniform sampler2D SHADOWMAP;

varying vec4 posE;	
varying vec3 normalE;
varying vec4 lightPosP;
uniform mat4 VIEW_MAT;


//parallel light
vec4 BlinnPhong(vec4 orign_color, vec3 dir_l, vec3 normal, vec3 v, sampler2D shadowMap, vec4 lPos)
{
	//ambient
	vec4 La = gl_LightSource[0].ambient;
		
	float visibility = 1.0;
	lPos = lPos / lPos.w;
	float bias = 0.001 * tan(acos( dot(normal, dir_l) ));
	bias = clamp(bias, 0.0, 0.01);
	if(texture2D(shadowMap, lPos.xy).x < lPos.z - bias)
	{
		visibility = 0;
	}

	//diffuse <N,L>*I
	float dot_value = max( dot( normal, dir_l), 0.0 );
	vec4 Ld = dot_value * gl_LightSource[0].diffuse;
				
	//specular <N,H>^n * I
	vec3 H = normalize( v + dir_l );
	dot_value = pow( max(dot(H, normal ), 0.0), 64);	
	vec4 Ls = gl_LightSource[0].specular * dot_value;

	return orign_color * min((La + (Ld + Ls) * visibility), 1.0);
}


void main(void)
{
	vec4 color = texture2D(TEX_COLOR, gl_TexCoord[0].st);	
	// parallel light
	vec3 dirL = normalize( VIEW_MAT * gl_LightSource[0].position).xyz;	

	gl_FragColor = BlinnPhong(color, dirL, normalize(normalE), normalize(-posE.xyz), SHADOWMAP, lightPosP);

}
