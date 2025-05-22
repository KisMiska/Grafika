#version 330 core
out vec4 FragColor;

in vec4 outCol;
in vec3 outNormal;
in vec3 outWorldPosition;

void main() {
    FragColor = outCol;
}