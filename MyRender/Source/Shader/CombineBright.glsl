@vertex shader
void main(void)
{
	gl_TexCoord[0] = gl_MultiTexCoord0;

	gl_Position = ftransform();
}

@fragment shader
uniform sampler2D TEX_COLOR;
uniform sampler2D BLUR;

const float gamma = 1.4;
const float exposure = 2;
void main(void)
{
	vec3 color = texture2D(TEX_COLOR, gl_TexCoord[0].st).rgb;	
	vec3 blur = texture2D(BLUR, gl_TexCoord[0].st).rgb;

	vec3 hdr = color + blur;
	// tone mapping
    vec3 result = vec3(1.0) - exp(-hdr * exposure);
    // also gamma correct while we're at it       
    result = pow(result, vec3(1.0 / gamma));

	gl_FragColor = vec4(result, 1.0);


}
