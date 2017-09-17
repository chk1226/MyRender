@vertex shader
#version 120 

void main(void)
{
	gl_TexCoord[0] = gl_MultiTexCoord0;

	gl_Position = ftransform();
}

@fragment shader
#version 120 

const int LigthNum = 63;
uniform vec4 LightInfo[LigthNum];	//x,y,z is poaition, coordinate at viewspace, w is radius
uniform vec3 LightColor[LigthNum];
uniform vec4 AttenuationInfo;	// constant, linear, quadratic, imax
uniform sampler2D TEX_COLOR;	
uniform sampler2D POSITION;	// coordinate at viewspace
uniform sampler2D NORMAL;	// coordinate at viewspace

float Attenuation(float dist)
{
	float attenuation = AttenuationInfo.w / (AttenuationInfo.x + AttenuationInfo.y * dist + 
    		    AttenuationInfo.z * (dist * dist)); 
	return attenuation;
}

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
	vec4 orig = texture2D(TEX_COLOR, gl_TexCoord[0].st);	
	vec4 color = orig;
	for(int i = 0; i < LigthNum; ++i)
    {
		vec4 posR = texture2D(POSITION, gl_TexCoord[0].st);	
		vec3 FragPos = posR.xyz;
		vec3 FragNormal = texture2D(NORMAL, gl_TexCoord[0].st).xyz;

        // calculate distance between light source and current fragment
		vec3 dir = LightInfo[i].xyz - FragPos;
		float radius = LightInfo[i].w;
        float d = length(dir);
        if(d <= radius)
        {
            color += BlinnPhong(vec4(LightColor[i], 1), normalize(dir), normalize(FragNormal), normalize(-FragPos)) * Attenuation(d);
        }
    } 

	gl_FragColor = vec4(color.rgb, orig.a);

}
