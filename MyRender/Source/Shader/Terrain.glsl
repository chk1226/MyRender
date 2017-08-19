@vertex shader
varying vec4 posE;	
varying vec3 normalE;	
varying float blendY;	

uniform mat4 ModelMatrix;
uniform vec4 ClipPlane;

void main(void)
{
	posE = gl_ModelViewMatrix * gl_Vertex;
    normalE = gl_NormalMatrix * gl_Normal;
	blendY = gl_Vertex.y;
	gl_TexCoord[0] = gl_MultiTexCoord0;
	gl_Position = ftransform();

	gl_ClipDistance[0] = dot(ModelMatrix * gl_Vertex, ClipPlane);
}

@fragment shader
uniform sampler2D TEX_COLOR;
uniform sampler2D TEX2_COLOR;

varying vec4 posE;	
varying vec3 normalE;
varying float blendY;	

uniform vec3 DIR_LIGHT;

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
	dot_value = pow( max(dot(H, normal ), 0.0), 0);	
	vec4 Ls = gl_LightSource[0].specular * dot_value;
	
	return orign_color * min((La + Ld + Ls * 0.4), 1.0);
}


void main(void)
{
	vec4 grass = texture2D(TEX_COLOR, gl_TexCoord[0].st);	
	vec4 soil = texture2D(TEX2_COLOR, gl_TexCoord[0].st);	

	float factor = 1;
	if(blendY < 1)
	{
		if(blendY > -1)
		{
			factor =  smoothstep(0, 1, (blendY + 1.0)/2);
		}
		else
		{
			factor = 0;
		}
	}

	vec4 color = grass * factor + soil * (1.0 - factor);


	// parallel light
	gl_FragColor = BlinnPhong(color, DIR_LIGHT, normalize(normalE), normalize(-posE.xyz));
	
}
