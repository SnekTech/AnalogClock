namespace AnalogClock;

public partial class Main : Node
{
    Vector2I _originalWindowSize;

    public override void _Ready()
    {
        _originalWindowSize = GetWindow().Size;
        GD.Print("Hello from pixel art template!");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton inputEventMouseButton)
        {
            const int windowSizeUpdateStep = 10;
            var windowSizeIncrement = inputEventMouseButton.ButtonIndex switch
            {
                MouseButton.WheelUp => new Vector2I(windowSizeUpdateStep, windowSizeUpdateStep),
                MouseButton.WheelDown => - new Vector2I(windowSizeUpdateStep, windowSizeUpdateStep),
                MouseButton.Middle => _originalWindowSize - GetWindow().Size,
                _ => Vector2I.Zero,
            };
            GetWindow().Size += windowSizeIncrement;
        }
    }
}