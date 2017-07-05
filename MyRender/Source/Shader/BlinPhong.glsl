@vertex shader
varying vec4 posE;	
varying vec3 normalE;	


void main(void)
{
	posE = gl_ModelViewMatrix * gl_Vertex;
    normalE = gl_NormalMatrix * gl_Normal;
	gl_TexCoord[0] = gl_MultiTexCoord0;

	gl_Position = ftransform();
}

@fragment shader
uniform sampler2D TEX_COLOR;

varying vec4 posE;	
varying vec3 normalE;


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
	dot_value = pow( max(dot(H, normal ), 0.0), gl_FrontMaterial.shininess);	
	vec4 Ls = gl_LightSource[0].specular * dot_value;
	
	return orign_color * min((La + Ld + Ls), 1.0);
}


void main(void)
{
	vec4 color = texture2D(TEX_COLOR, gl_TexCoord[0].st);	
	// parallel light
	vec3 dirL = normalize( gl_LightSource[0].position.xyz);	

	gl_FragColor = BlinnPhong(color, dirL, normalize(normalE), normalize(-posE.xyz));

	
}
