@vertex shader
varying vec4 posP;
varying vec4 reflectionPosP;
varying vec3 ToCamera;
varying vec3 ToLight;

uniform mat4 REFLECTION_PVM;
uniform mat4 ModelMatrix;
uniform vec3 CameraPos;
uniform vec3 LightPos;

const float textureScale = 5;

void main(void)
{
    posP = gl_ModelViewProjectionMatrix * gl_Vertex;
	gl_TexCoord[0] = gl_MultiTexCoord0 * textureScale;
	reflectionPosP = REFLECTION_PVM * gl_Vertex;
	ToCamera = CameraPos - (ModelMatrix * gl_Vertex).xyz;
	ToLight = (ModelMatrix * gl_Vertex).xyz - LightPos;
	gl_Position = ftransform();

}

@fragment shader
uniform sampler2D REFLECTION;
uniform sampler2D REFRACTION;
uniform sampler2D DUDVMAP;
uniform float MoveFactor;
uniform sampler2D NORMAL_TEX_COLOR;
uniform sampler2D REFRACTION_DEPTH;
uniform float Near;
uniform float Far;

varying vec4 posP;
varying vec4 reflectionPosP;
varying vec3 ToCamera;
varying vec3 ToLight;

const float waveStrength = 0.005;
float GetLinearDepth(float depth)
{
	return 2.0 * Near * Far / (Far + Near - (2.0 * depth - 1.0) * (Far - Near));
}

void main(void)
{
    vec4 posProject = posP / posP.w;
    posProject = posProject / 2 + 0.5;
	vec4 rPosP = reflectionPosP / reflectionPosP.w;
	rPosP = rPosP / 2 + 0.5;

	// calculation floor to surface
	float floorToSurface = GetLinearDepth(texture2D(REFRACTION_DEPTH, posProject.xy).r) -  GetLinearDepth(gl_FragCoord.z);

	vec2 distor = texture2D(DUDVMAP, gl_TexCoord[0].st + vec2(MoveFactor, 0)).rg * 0.01;
	distor = gl_TexCoord[0].st + vec2(distor.x - MoveFactor, distor.y + MoveFactor);
	vec2 total = (texture2D(DUDVMAP, distor).xy * 2.0 - 1.0) * waveStrength * clamp(floorToSurface / 5, 0.0, 1.0);

	vec4 reflection = texture2D(REFLECTION, rPosP.xy + total);	
	// clamp is prevent glitch
	vec4 refraction = texture2D(REFRACTION, clamp(posProject.xy + total, 0.001, 0.999));	

	// Fresnel Effect
	float refractionFactor = dot(normalize(ToCamera), vec3(0, 1, 0));
	// pow is tweak value
	vec4 color = mix(reflection, refraction, refractionFactor);
	color = mix(color, vec4(0, 0.3, 0.5, 1), 0.1);

	// specular
	vec3 normal_map = texture2D(NORMAL_TEX_COLOR, distor).xyz;
	normal_map = vec3(normal_map.r * 2.0 - 1.0, normal_map.b, normal_map.g*2.0 - 1.0);
	normal_map = normalize(normal_map);

	float dot_value = max(dot( reflect(normalize(ToLight), normal_map), normalize(ToCamera)), 0.0);
	dot_value = pow(dot_value, 64);
	vec4 Ls = gl_LightSource[0].specular * dot_value * 0.7 * clamp(floorToSurface / 5, 0.0, 1.0);

	vec4 result = color + Ls;
	result.a = clamp(floorToSurface / 5, 0.0, 1.0);
    gl_FragColor = result;

}
