@vertex shader
void main(void)
{
	gl_TexCoord[0] = gl_MultiTexCoord0;

	gl_Position = ftransform();
}

@fragment shader
uniform sampler2D TEX_COLOR;
uniform sampler2D BLUR;
uniform sampler2D DEPTH;
uniform float Near;
uniform float Far;
uniform float Zoom;

const float focus = 10.0;
const float maxZoom = 30.0;
const float minZoom = 20.0;

float GetLinearDepth(float depth)
{
	return 2.0 * Near * Far / (Far + Near - (2.0 * depth - 1.0) * (Far - Near));
}

void main(void)
{
	vec3 color = texture2D(TEX_COLOR, gl_TexCoord[0].st).rgb;	
	vec3 blur = texture2D(BLUR, gl_TexCoord[0].st).rgb;	

	float depth = texture2D(DEPTH, gl_TexCoord[0].st).r;

	float f = clamp( (GetLinearDepth(depth) - focus) / focus, 0.0, 1.0);
	if(Zoom > maxZoom)
	{
		f = 0.0;
	}
	else if(Zoom >= minZoom && Zoom <= maxZoom)
	{
		f = smoothstep(0, f, (maxZoom - Zoom) / (maxZoom - minZoom));
	}

	color = mix(color, blur, f);
	gl_FragColor = vec4(color, 1.0);


}
