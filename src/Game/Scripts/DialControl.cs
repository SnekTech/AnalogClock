using GodotGadgets.Extensions;

namespace AnalogClock;

[SceneTree]
public partial class DialControl : Control
{
    DialShader _dialShader = null!;

    public override void _Ready()
    {
        _dialShader = new DialShader(_.DialRect.GetMaterialAs<ShaderMaterial>());
        UpdateDialShader();

        _.ClockTimer.Start();
    }

    public override void _EnterTree()
    {
        _.ClockTimer.Timeout += OnSyncTimerTimeOut;
    }

    public override void _ExitTree()
    {
        _.ClockTimer.Timeout -= OnSyncTimerTimeOut;
    }

    void OnSyncTimerTimeOut()
    {
        UpdateDialShader();
    }

    void UpdateDialShader()
    {
        var shaderInput = DialShaderInput.FromTimeOnly(TimeOnly.FromDateTime(DateTime.Now));
        _dialShader.Update(shaderInput);
    }
}