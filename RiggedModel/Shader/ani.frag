#version 420 core

out vec4 out_colour;

in vec2 pass_textureCoords;
in vec3 pass_normals;
in vec3 pass_weights;

uniform sampler2D diffuseMap;

void main(void)
{
	vec4 diffuseColour = texture(diffuseMap, pass_textureCoords);	
	out_colour = diffuseColour; //vec4(pass_weights, 1.0); //vec4(1.0, 1.0, 0.0, 1.0);
}