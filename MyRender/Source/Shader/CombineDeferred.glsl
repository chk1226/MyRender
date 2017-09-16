@vertex shader
#version 120 

void main(void)
{
	gl_TexCoord[0] = gl_MultiTexCoord0;
	gl_Position = ftransform();
}

@fragment shader
#version 120 

uniform sampler2D BACK;
uniform sampler2D MID;
uniform sampler2D FRONT;
void main(void)
{
	vec4 back = texture2D(BACK, gl_TexCoord[0].st).rgba;	
	vec4 mid = texture2D(MID, gl_TexCoord[0].st).rgba;
	vec4 front = texture2D(FRONT, gl_TexCoord[0].st).rgba;

	if(mid.a != 0)
	{
		back = mid;
	}

	if(front.a != 0)
	{
		back = front;
	}

	gl_FragColor = back;

}
