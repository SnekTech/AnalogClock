using GodotGadgets.Extensions;

namespace AnalogClock;

[SceneTree]
public partial class Dial : Sprite2D
{
    DialShader _dialShader = null!;

    public override void _Ready()
    {
        _dialShader = new DialShader(this.GetMaterialAs<ShaderMaterial>());
        UpdateDialShader();

        _.Timer.Start();
    }

    public override void _EnterTree()
    {
        _.Timer.Timeout += OnSyncTimerTimeOut;
    }

    public override void _ExitTree()
    {
        _.Timer.Timeout -= OnSyncTimerTimeOut;
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