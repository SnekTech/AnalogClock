namespace AnalogClock.Components;

record DialDigit(int Hour);

static class DialDigitExtensions
{
    extension(DialDigit dialDigit)
    {
        internal string DialText => (dialDigit.Hour % 12) switch
        {
            0 => "12",
            _ => (dialDigit.Hour % 12).ToString(),
        };

        internal Vector2 GetDialDirection()
        {
            var steps = dialDigit.Hour % 12;
            var angle = -Mathf.Pi / 2f + steps * Mathf.Tau / 12f;
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }
    }
}