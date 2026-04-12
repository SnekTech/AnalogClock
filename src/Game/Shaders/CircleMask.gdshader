shader_type canvas_item;

uniform float u_radius: hint_range(0, 1) = 1.0;

void fragment()
{
    vec2 uv = UV * 2.0 - 1.0;
    float circleMask = step( length( uv ), u_radius );
    COLOR.a = COLOR.a * circleMask;
}