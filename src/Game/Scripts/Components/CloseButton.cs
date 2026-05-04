using GodotGadgets.TweenStuff;
using GTweens.Easings;
using GTweensGodot.Extensions;

namespace AnalogClock.Components;

public partial class CloseButton : Button
{
    readonly SingleTweenPlayer _singleTweenPlayer = new();

    const float VisibleAlpha = 1f;
    const float HiddenAlpha = 0f;
    const float FadeDuration = 0.2f;

    public override void _Ready()
    {
        Modulate = Modulate with { A = HiddenAlpha };
    }

    public override void _EnterTree()
    {
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
        Pressed += OnPressed;
    }

    public override void _ExitTree()
    {
        MouseEntered -= OnMouseEntered;
        MouseExited -= OnMouseExited;
        Pressed -= OnPressed;

        _singleTweenPlayer.Dispose();
    }

    void OnPressed()
    {
        GetTree().Quit();
    }

    void OnMouseEntered()
    {
        var tween = this.TweenModulateAlpha(VisibleAlpha, FadeDuration)
            .SetEasing(Easing.OutCubic);
        _singleTweenPlayer.KillPreviousAndPlay(tween);
    }

    void OnMouseExited()
    {
        var tween = this.TweenModulateAlpha(HiddenAlpha, FadeDuration)
            .SetEasing(Easing.OutCubic);
        _singleTweenPlayer.KillPreviousAndPlay(tween);
    }
}