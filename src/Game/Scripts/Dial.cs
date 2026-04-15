using GodotGadgets.Extensions;

namespace AnalogClock;

public partial class Dial : Sprite2D
{
    DialShader _dialShader = null!;

    public override void _Ready()
    {
        _dialShader = new DialShader(this.GetMaterialAs<ShaderMaterial>());
    }

    public override void _Process(double delta)
    {
        var shaderInput = DialShaderInput.FromDateTime(DateTime.UtcNow);
        _dialShader.Update(shaderInput);
    }
}