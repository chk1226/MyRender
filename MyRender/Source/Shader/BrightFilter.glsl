@vertex shader
void main(void)
{
	gl_TexCoord[0] = gl_MultiTexCoord0;

	gl_Position = ftransform();
}

@fragment shader
uniform sampler2D TEX_COLOR;

void main(void)
{
	vec4 color = texture2D(TEX_COLOR, gl_TexCoord[0].st);	
	float brightness = dot(color.rgb, vec3(0.2126, 0.7152, 0.0722));

    if(brightness > 0.8)
        gl_FragColor = color;
	else
		gl_FragColor = vec4(0,0,0,0);

	
}
