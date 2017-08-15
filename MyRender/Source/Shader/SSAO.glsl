@vertex shader
void main(void)
{
	gl_TexCoord[0] = gl_MultiTexCoord0;
	
	gl_Position = ftransform();
}

@fragment shader

uniform sampler2D GPOSITION;
uniform sampler2D GNORMAL;
uniform sampler2D NOISE;
uniform vec3 SAMPLES[64];
uniform mat4 PROJECTION;
uniform vec2 NOISE_SCALE;

const int kernelSize = 32;
const float radius = 1.00;
const float bias = 0.025;

void main(void)
{				
	vec3 gPos   = texture2D(GPOSITION, gl_TexCoord[0].st).xyz;
	vec3 gNormal    = texture2D(GNORMAL, gl_TexCoord[0].st).rgb;
	vec3 randomVec = texture2D(NOISE, gl_TexCoord[0].st * NOISE_SCALE).xyz;  

	vec3 tangent   = normalize(randomVec - gNormal * dot(randomVec, gNormal));
	vec3 bitangent = cross(gNormal, tangent);
	mat3 TBN       = mat3(tangent, bitangent, gNormal);  

	float occlusion = 0.0;
	vec3 sampleE;
	for(int i = 0; i < kernelSize; ++i)
	{
		// get sample position
		sampleE = TBN * SAMPLES[i]; // From tangent to view-space
		sampleE = gPos + sampleE * radius; 
    
		vec4 offset = vec4(sampleE, 1.0);
		offset      = PROJECTION * offset;    // from view to clip-space
		offset.xyz /= offset.w;               // perspective divide
		offset.xyz  = offset.xyz * 0.5 + 0.5; // transform to range 0.0 - 1.0  

		float sampleDepth = texture2D(GPOSITION, offset.xy).z; 
		float rangeCheck = smoothstep(0.0, 1.0, radius / abs(sampleE.z - sampleDepth));

		if(sampleDepth >= sampleE.z + bias)
		{
			occlusion += (1.0 * rangeCheck);
		}

	} 

	occlusion = 1.0 - (occlusion / kernelSize);
	gl_FragColor = vec4(occlusion, 0, 0, 1);
	
}