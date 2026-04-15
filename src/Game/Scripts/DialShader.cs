using GodotGadgets.ShaderStuff;

namespace AnalogClock;

class DialShader(ShaderMaterial shaderMaterial)
{
    readonly Uniform<float> _hourHandRads = shaderMaterial.GetUniform<float>("u_hour_hand_rads");
    readonly Uniform<float> _minuteHandRads = shaderMaterial.GetUniform<float>("u_minute_hand_rads");
    readonly Uniform<float> _secondHandRads = shaderMaterial.GetUniform<float>("u_second_hand_rads");

    internal void Update(DialShaderInput dialShaderInput)
    {
        var (hourAngle, minuteAngle, secondAngle) = dialShaderInput;
        _hourHandRads.Value = hourAngle;
        _minuteHandRads.Value = minuteAngle;
        _secondHandRads.Value = secondAngle;
    }
}

record DialShaderInput(float HourHandRads, float MinuteHandRads, float SecondHandRads);

static class DialShaderInputExtensions
{
    extension(DialShaderInput)
    {
        internal static DialShaderInput FromTimeOnly(TimeOnly timeOnly)
        {
            var (hour, minute, second) = (timeOnly.Hour % 12, timeOnly.Minute, timeOnly.Second);
            const float secondsInTwelveHours = 12 * 3600;
            const float secondsInOneHour = 3600;
            var totalSeconds = GetTotalSeconds(hour, minute, second);
            var hourAngle = totalSeconds / secondsInTwelveHours * Mathf.Tau;

            var secondsInThisHour = minute * 60 + second;
            var minuteAngle = secondsInThisHour / secondsInOneHour * Mathf.Tau;

            var secondAngle = second / 60f * Mathf.Tau;

            return new DialShaderInput(hourAngle, minuteAngle, secondAngle);

            static int GetTotalSeconds(int hour, int minute, int seconds)
            {
                return hour * 3600 + minute * 60 + seconds;
            }
        }
    }
}