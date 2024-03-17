#version 420 core

out vec4 out_colour;
in vec4 pass_weights;

void main(void)
{
	out_colour = vec4(pass_weights.xyz, 1.0f);
	//out_colour = vec4(1,0,0,1);
}