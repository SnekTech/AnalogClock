namespace AnalogClock.Components;

[SceneTree]
public partial class DigitLabel : Label
{
    internal DialDigit Digit
    {
        get;
        set
        {
            field = value;
            Text = field.DialText;
        }
    } = new(12);
}
