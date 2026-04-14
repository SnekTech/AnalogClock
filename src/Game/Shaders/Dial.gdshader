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

group_uniforms dial_hand;
uniform float u_hour_hand_progress: hint_range(0, 1);
uniform float u_minute_hand_progress: hint_range(0, 1);
uniform float u_second_hand_progress: hint_range(0, 1);
uniform vec3 u_hour_hand_color: source_color;
uniform vec3 u_second_hand_color: source_color;
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

float DrawLineSegment(vec2 p, vec2 point_a, vec2 point_b, float width)
{
    vec2 ab = point_b - point_a;
    vec2 ap = p - point_a;
    float ab_sq = dot( ab, ab );
    
    float t = 0.0; // 避免除以 0
    if ( ab_sq > 0.0 )
    {
        t = dot( ap, ab ) / ab_sq;
        t = Clamp01( t );
    }
    
    vec2 projection = point_a + ab * t;
    float dist = distance( p, projection );
    
    return step( dist, width );
}

float DrawMarkingAtAngle(vec2 uv, float angle, float width, float length, float padding)
{
    vec2 angleDirection = vec2( cos( angle ), sin( angle ) );
    vec2 markingStartPoint = angleDirection * ( 1.0 - padding - length );
    vec2 markingEndPoint = angleDirection * ( 1.0 - padding );
    return DrawLineSegment( uv, markingStartPoint, markingEndPoint, width );
}

float DrawHand(vec2 uv, float angle, float handLength, float handWidth)
{
    angle += -PI / 2.0;
    vec2 angleDirection = vec2( cos( angle ), sin( angle ) );
    vec2 handEndPoint = angleDirection * handLength;
    return DrawLineSegment( uv, vec2( 0,0 ), handEndPoint, handWidth );
}

float DrawHourHand(vec2 uv)
{
    const float HourHandLength = 0.6;
    const float HourHandWidth = 0.02;
    return DrawHand( uv, u_hour_hand_progress * TAU, HourHandLength, HourHandWidth );
}

float DrawMinuteHand(vec2 uv)
{
    const float MinuteHandLength = 1.0 - MarkingPaddingMax - 0.1;
    const float MinuteHandWidth = 0.02;
    return DrawHand( uv, u_minute_hand_progress * TAU, MinuteHandLength, MinuteHandWidth );
}

float DrawSecondHand(vec2 uv)
{
    return DrawHand( uv, u_second_hand_progress, 0.9, MajorMarkingWidthMax );
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
    float minorStep = TAU / 60.0;
    float minorMarkingWidth = MinorMarkingWidthMax * u_minor_marking_width;
    float minorMarkingLength = MinorMarkingLengthMax * u_minor_marking_length;
    float minorMarkingMask = 0.0;
    for ( float targetAngle = 0.0; targetAngle < TAU; targetAngle += minorStep )
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
    
    float hourHandMask = DrawHourHand( uv );
    float minuteHandMask = DrawMinuteHand( uv );
    float secondHandMask = DrawSecondHand( uv );
    
    COLOR.rgb = mix( COLOR.rgb, u_minor_marking_color, minorMarkingMask );
    COLOR.rgb = mix( COLOR.rgb, u_major_marking_color, majorMarkingMask );
    
    COLOR.rgb = mix( COLOR.rgb, u_hour_hand_color, hourHandMask );
    COLOR.rgb = mix( COLOR.rgb, u_hour_hand_color, minuteHandMask );
    COLOR.rgb = mix( COLOR.rgb, u_second_hand_color, secondHandMask );
}