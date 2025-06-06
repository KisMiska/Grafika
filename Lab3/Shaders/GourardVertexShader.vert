#version 330 core
layout (location = 0) in vec3 vPos;
layout (location = 1) in vec4 vCol;
layout (location = 2) in vec3 vNormal;

uniform mat4 uModel;
uniform mat3 uNormal;
uniform mat4 uView;
uniform mat4 uProjection;

uniform vec3 uLightColor;
uniform vec3 uLightPos;
uniform vec3 uViewPos;
uniform float uShininess;

out vec4 outCol;
out vec3 outNormal;
out vec3 outWorldPosition;

void main() {
    vec3 worldPos = vec3(uModel * vec4(vPos.x, vPos.y, vPos.z, 1.0));
    vec3 norm = normalize(uNormal * vNormal);

    float ambientStrength = 0.1;
    vec3 ambient = ambientStrength * uLightColor;
    
    float diffuseStrength = 0.3;
    vec3 lightDir = normalize(uLightPos - worldPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = diff * uLightColor * diffuseStrength;
    
    float specularStrength = 0.6;
    vec3 viewDir = normalize(uViewPos - worldPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), uShininess);
    vec3 specular = specularStrength * spec * uLightColor;
    
    vec3 result = (ambient + diffuse + specular) * vCol.rgb;
    
    outCol = vec4(result, vCol.a);
    outNormal = norm;
    outWorldPosition = worldPos;
    
    gl_Position = uProjection * uView * uModel * vec4(vPos.x, vPos.y, vPos.z, 1.0);
}