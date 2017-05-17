@vertex shader
void main(void)
{
	gl_Position = ftransform();
}

@fragment shader
void main(void)
{	
	gl_FragColor = vec4(1.0, 0.68, 0.78, 1.0);	
}
