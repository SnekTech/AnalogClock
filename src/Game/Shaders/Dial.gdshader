shader_type canvas_item;

uniform float u_radius: hint_range(0, 1) = 1.0;
uniform vec3 u_background_color: source_color;
uniform float u_marking_padding: hint_range(0, 1) = 0.5;

group_uniforms minor_marking;
uniform float u_minor_marking_length: hint_range(0, 1) = 0.5;
uniform float u_minor_marking_width: hint_range(0, 1) = 0.5;
uniform vec3 u_minor_marking_color: source_color;
group_uniforms;

group_uniforms major_marking;
uniform float u_major_marking_length: hint_range(0, 1) = 0.5;
uniform float u_major_marking_width: hint_range(0, 1) = 0.5;
uniform vec3 u_major_marking_color: source_color;
group_uniforms;

const float MarkingPaddingMax = 0.05;
const float MinorMarkingLengthMax = 0.05;
const float MinorMarkingWidthMax = TAU / 360.0;

const float MajorMarkingLengthMax = 0.07;
const float MajorMarkingWidthMax = 0.01;

float Clamp01(float x)
{
    return clamp( x, 0., 1. );
}

float ModAngle(float angle)
{
    return mod( angle + TAU, TAU );
}

float GetDialAngle(vec2 uv)
{
    float dial = atan( uv.y, -uv.x );
    return ModAngle( dial );
}

float IsBetween(float x, float min, float max)
{
    return step( min, x ) * step( x, max );
}

float DrawMarkingAtAngle(vec2 uv, float angle, float width, float length, float padding)
{
    float slope = tan( angle + PI / 2.0 );
    float line = uv.y - slope * uv.x;
    float distanceToLine = abs( line ) / sqrt( 1. + slope * slope );
    float lineMask = step( distanceToLine, width );
    
    float distanceFromOrigin = length( uv );
    float minorMarkingMask = lineMask * IsBetween( distanceFromOrigin, 1.0 - padding - length, 1.0 - padding );
    return minorMarkingMask;
}

void fragment()
{
    vec2 uv = UV * 2.0 - 1.0;
    float distanceFromOrigin = length( uv );
    
    // bg shape
    float backgroundCircleMask = step( distanceFromOrigin, u_radius );
    COLOR.a = COLOR.a * backgroundCircleMask;
    
    // bg color
    COLOR.rgb = mix( COLOR.rgb, u_background_color, backgroundCircleMask );
    
    // minor markings
    float markingPadding = MarkingPaddingMax * u_marking_padding;
    float step = TAU / 60.0;
    float minorMarkingWidth = MinorMarkingWidthMax * u_minor_marking_width;
    float minorMarkingLength = MinorMarkingLengthMax * u_minor_marking_length;
    float minorMarkingMask = 0.0;
    for ( float targetAngle = 0.0; targetAngle < TAU; targetAngle += step )
    {
        minorMarkingMask += DrawMarkingAtAngle( uv, targetAngle, minorMarkingWidth, minorMarkingLength, markingPadding );
    }
    minorMarkingMask = Clamp01( minorMarkingMask );
    
    // major markings
    float majorStep = TAU / 12.0;
    float majorMarkingWidth = MajorMarkingWidthMax * u_major_marking_width;
    float majorMarkingLength = MajorMarkingLengthMax * u_major_marking_length;
    float majorMarkingMask = 0.0;
    for ( float markingAngle = 0.0; markingAngle < TAU; markingAngle += majorStep )
    {
        majorMarkingMask += DrawMarkingAtAngle( uv, markingAngle, majorMarkingWidth, majorMarkingLength, markingPadding );
    }
    majorMarkingMask = Clamp01( majorMarkingMask );
    
    
    COLOR.rgb = mix( COLOR.rgb, u_minor_marking_color, minorMarkingMask );
    COLOR.rgb = mix( COLOR.rgb, u_major_marking_color, majorMarkingMask );
}