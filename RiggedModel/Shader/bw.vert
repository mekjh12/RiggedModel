#version 420 core

const int MAX_JOINTS = 128;
const int MAX_WEIGHTS = 3;

layout(location = 0) in vec3 in_position;
layout(location = 1) in vec2 in_textureCoords;
layout(location = 2) in vec3 in_normal;
layout(location = 3) in vec4 in_jointIndices;
layout(location = 4) in vec4 in_weights;

out vec4 pass_weights;

uniform mat4 proj;
uniform mat4 view;
uniform mat4 model;
uniform int boneIndex;

void main(void)
{	
	gl_Position = proj * view * model * vec4(in_position, 1.0);

	 float weight = 0.0f;
	if (in_jointIndices.x == boneIndex) weight += in_weights.x;
	if (in_jointIndices.y == boneIndex) weight += in_weights.y;
	if (in_jointIndices.z == boneIndex) weight += in_weights.z;
	if (in_jointIndices.w == boneIndex) weight += in_weights.w;

	if (weight>=0.0f && weight<0.5f)
	{
		pass_weights = vec4(0, 2*weight, 1-2*weight, 1);
    }
	else if (weight>=0.5f && weight<1.0f)
	{
		pass_weights = vec4(2*weight-1,-2*weight+2, 0, 1);
	}
	else if (weight>=1.0f)
	{
		pass_weights = vec4(1, 0, 0, 1);
	}
	else
	{
		pass_weights = vec4(0, 0, 0, 1);
	}

}