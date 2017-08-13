@vertex shader
varying vec4 posE;	
varying vec3 normalE;	
varying vec4 lightPosP;

uniform mat4 LIGHT_BPVM;

void main(void)
{
	posE = gl_ModelViewMatrix * gl_Vertex;
    normalE = gl_NormalMatrix * gl_Normal;
	gl_TexCoord[0] = gl_MultiTexCoord0;
	lightPosP = LIGHT_BPVM * gl_Vertex;

	gl_Position = ftransform();
}

@fragment shader
uniform sampler2D TEX_COLOR;
uniform sampler2D SHADOWMAP;
uniform vec3 DIR_LIGHT;

varying vec4 posE;	
varying vec3 normalE;
varying vec4 lightPosP;


float chebyshevUpperBound(sampler2D shadowMap, vec4 lPos, float bias)
{
	// We retrive the two moments previously stored (depth and depth*depth)
	vec2 moments = texture2D(shadowMap, lPos.xy).rg;
	
	// Surface is fully lit. as the current fragment is before the light occluder
	if (lPos.z - bias <= moments.x)
		return 1.0 ;
	
	// The fragment is either in shadow or penumbra. We now use chebyshev's upperBound to check
	// How likely this pixel is to be lit (p_max)
	float variance = moments.y - (moments.x * moments.x);
	variance = max(variance, 0.00002);
	
	float d = (lPos.z - bias) - moments.x;
	float p_max = variance / (variance + d*d);
	
	return p_max;
}

//parallel light
vec4 BlinnPhong(vec4 orign_color, vec3 dir_l, vec3 normal, vec3 v, sampler2D shadowMap, vec4 lPos)
{
	//ambient
	vec4 La = gl_LightSource[0].ambient;
		
	lPos = lPos / lPos.w;
	// bias
	float bias = 0.001 * tan(acos( dot(normal, dir_l) ));
	bias = clamp(bias, 0.0, 0.01);
	float visibility = chebyshevUpperBound(shadowMap, lPos, bias);
	//float visibility = 0;
	//if(texture2D(SHADOWMAP, lPos.xy).x >= lPos.z - bias)
	//{
	//	visibility = 1;
	//}



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
	gl_FragColor = BlinnPhong(color, DIR_LIGHT, normalize(normalE), normalize(-posE.xyz), SHADOWMAP, lightPosP);
}
