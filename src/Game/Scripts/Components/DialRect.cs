using GodotGadgets.Extensions;

namespace AnalogClock.Components;

[SceneTree]
public partial class DialRect : TextureRect
{
    readonly List<DigitLabel> _digitLabels = [];

    public override void _Ready()
    {
        this.ClearChildren();
        CreateDigitLabels();
        UpdateDigitLabelPosition();
    }

    void CreateDigitLabels()
    {
        for (var i = 0; i < 12; i++)
        {
            var digit = new DialDigit(i);
            var label = DigitLabel.InstantiateOnParent(this);
            label.Digit = digit;
            _digitLabels.Add(label);
        }
    }

    void UpdateDigitLabelPosition()
    {
        const float radiusRatio = 0.4f;
        var rect = GetGlobalRect();
        var radius = Mathf.Min(rect.Size.X, rect.Size.Y) * radiusRatio;
        var center = rect.GetCenter();
        foreach (var digitLabel in _digitLabels)
        {
            digitLabel.GlobalPosition = center + digitLabel.Digit.GetDialDirection() * radius - digitLabel.Size / 2;
        }
    }
}