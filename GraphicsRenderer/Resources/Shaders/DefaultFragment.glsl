#version 400

in vec2 texCoord;

uniform sampler2D _texture;

out vec4 FragColor;

void main()
{
    FragColor = vec4(1, 1, 1, 1);  // texture(_texture, texCoord);
}