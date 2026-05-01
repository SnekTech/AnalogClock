using GTweens.Builders;
using GTweens.Easings;
using GTweens.Enums;
using GTweens.Tweens;
using GTweensGodot.Extensions;

namespace AnalogClock.DragMechanic;

public class DragAnimator(TextureRect clockTexture)
{
    const float ShortAnim = 0.1f;
    const float NormalAnim = 0.3f;

    public event Action? PickupEnded;

    GTween? _currentTween;

    public void PlayIdle()
    {
        KillCurrentTween();

        var tween = GTweenSequenceBuilder.New()
            .Join(clockTexture.TweenScale(Vector2.One, NormalAnim))
            .Join(clockTexture.TweenRotation(0f, NormalAnim))
            .Join(clockTexture.TweenModulate(
                new Color(1, 1, 1, 0.8f),
                NormalAnim
            ))
            .Build();

        tween.SetEasing(Easing.OutBack);
        tween.Play();

        _currentTween = tween;
    }

    public void PlayHover()
    {
        KillCurrentTween();

        var tween = GTweenSequenceBuilder.New()
            .Join(clockTexture.TweenScale(Vector2.One * 1.05f, 0.2f))
            .Build();
        tween.SetEasing(Easing.OutBack);
        tween.Play();
        _currentTween = tween;
    }

    public void PlayPickupStart()
    {
        KillCurrentTween();

        var randomRot = (float)GD.RandRange(-0.087, 0.087);

        var tween = GTweenSequenceBuilder.New()
            .Join(clockTexture.TweenScale(new Vector2(1.2f, 1.2f), ShortAnim).SetEasing(Easing.OutElastic))
            .Join(clockTexture.TweenRotation(randomRot, ShortAnim).SetEasing(Easing.OutBack))
            .Join(clockTexture.TweenModulate(new Color(1.3f, 1.3f, 1.3f), ShortAnim))
            .AppendCallback(() => PickupEnded?.Invoke())
            .Build();

        tween.Play();
        _currentTween = tween;
    }

    public void PlayHolding()
    {
        KillCurrentTween();

        // 循环浮动效果，作为视觉提示
        var tween = clockTexture.TweenPositionY(clockTexture.Position.Y - 3f, 0.4f)
            .SetEasing(Easing.InOutSine)
            .SetMaxLoops(ResetMode.PingPong);

        tween.Play();
        _currentTween = tween;
    }

    public void PlayDrop()
    {
        // KillCurrentTween();
        //
        // var tween = GTween.Sequence();
        // tween.Join(_clockTexture.GTweenScale(Vector2.One, NormalAnim)
        //     .SetEase(Easing.OutElastic));
        // tween.Join(_clockTexture.GTweenRotation(0f, 0.4f)
        //     .SetEase(Easing.OutBack));
        // tween.Join(_clockTexture.GTweenProperty("modulate", Colors.White, NormalAnim));
        //
        // tween.Play();
    }

    void KillCurrentTween()
    {
        _currentTween?.Kill();
        _currentTween = null;
    }
}