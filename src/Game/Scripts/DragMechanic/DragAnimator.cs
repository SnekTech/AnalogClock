using GodotGadgets.TweenStuff;
using GTweens.Builders;
using GTweens.Easings;
using GTweens.Enums;
using GTweensGodot.Extensions;

namespace AnalogClock.DragMechanic;

public class DragAnimator(TextureRect clockTexture) : IDisposable
{
    const float ShortAnim = 0.1f;
    const float NormalAnim = 0.3f;

    public event Action? PickupEnded;

    readonly SingleTweenPlayer _singleTweenPlayer = new();

    public void Dispose() => _singleTweenPlayer.Dispose();

    public void PlayIdle()
    {
        var tween = GTweenSequenceBuilder.New()
            .Join(clockTexture.TweenPositionY(0, ShortAnim))
            .Join(clockTexture.TweenScale(Vector2.One, NormalAnim))
            .Join(clockTexture.TweenRotation(0f, NormalAnim))
            .Join(clockTexture.TweenModulate(
                new Color(1, 1, 1, 0.8f),
                NormalAnim
            ))
            .Build();
        tween.SetEasing(Easing.OutBack);

        _singleTweenPlayer.KillPreviousAndPlay(tween);
    }

    public void PlayHover()
    {
        var tween = GTweenSequenceBuilder.New()
            .Join(clockTexture.TweenScale(Vector2.One * 1.05f, 0.2f))
            .Build();
        tween.SetEasing(Easing.OutBack);
        _singleTweenPlayer.KillPreviousAndPlay(tween);
    }

    public void PlayPickupStart()
    {
        var randomRot = (float)GD.RandRange(-0.087, 0.087);

        var tween = GTweenSequenceBuilder.New()
            .Join(clockTexture.TweenScale(new Vector2(1.2f, 1.2f), ShortAnim).SetEasing(Easing.OutElastic))
            .Join(clockTexture.TweenRotation(randomRot, ShortAnim).SetEasing(Easing.OutBack))
            .Join(clockTexture.TweenModulate(new Color(1.3f, 1.3f, 1.3f), ShortAnim))
            .AppendCallback(() => PickupEnded?.Invoke())
            .Build();

        _singleTweenPlayer.KillPreviousAndPlay(tween);
    }

    public void PlayHolding()
    {
        // 循环浮动效果，作为视觉提示
        var tween = clockTexture.TweenPositionY(clockTexture.Position.Y - 3f, 0.4f)
            .SetEasing(Easing.InOutSine)
            .SetMaxLoops(ResetMode.PingPong);

        _singleTweenPlayer.KillPreviousAndPlay(tween);
    }
}