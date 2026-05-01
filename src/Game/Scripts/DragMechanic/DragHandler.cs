namespace AnalogClock.DragMechanic;

public partial class DragHandler : Node
{
    [Export]
    DialControl dialControl = null!;
    
    Control _dragArea = null!;

    readonly DragStateMachine _stateMachine = new();
    DragStateMachine.IBinding _stateBinding = null!;
    DragAnimator _dragAnimator = null!;

    Vector2I _dragStartMousePos;
    Vector2I _windowStartPos;
    bool _isDragging;

    public override void _Ready()
    {
        SetupDialDragEffects();
        _stateMachine.Start();
        return;

        void SetupDialDragEffects()
        {
            _stateBinding = _stateMachine.Bind();
            _stateBinding.Handle((in DragStateMachine.Output.EnteredIdle _) => _dragAnimator.PlayIdle());
            _stateBinding.Handle((in DragStateMachine.Output.EnteredHover _) => _dragAnimator.PlayHover());
            _stateBinding.Handle((in DragStateMachine.Output.EnteredPickup _) => _dragAnimator.PlayPickupStart());
            _stateBinding.Handle((in DragStateMachine.Output.EnteredHolding _) => _dragAnimator.PlayHolding());
        }
    }

    public override void _EnterTree()
    {
        _dragAnimator = new DragAnimator(dialControl.TextureRect);
        _dragArea = dialControl.DragArea;

        _dragArea.MouseEntered += OnDragAreaMouseEntered;
        _dragArea.MouseExited += OnDragAreaMouseExited;
        _dragArea.GuiInput += OnGuiInput;
        _dragAnimator.PickupEnded += OnDragAnimatorPickupEffectEnded;
    }

    public override void _ExitTree()
    {
        _dragArea.MouseEntered -= OnDragAreaMouseEntered;
        _dragArea.MouseExited -= OnDragAreaMouseExited;
        _dragArea.GuiInput -= OnGuiInput;
        _dragAnimator.PickupEnded -= OnDragAnimatorPickupEffectEnded;
        
        _stateBinding.Dispose();
        _stateMachine.Stop();
    }

    void OnGuiInput(InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseButton { ButtonIndex: MouseButton.Left } eventMouseButton)
        {
            if (eventMouseButton.Pressed)
            {
                _stateMachine.Input_MousePressed();
                StartWindowDrag();
            }
            else
            {
                _stateMachine.Input_MouseReleased();
                StopWindowDrag();
            }
        }
    }

    void StartWindowDrag()
    {
        var window = GetWindow();
        _dragStartMousePos = DisplayServer.MouseGetPosition();
        _windowStartPos = window.Position;
        _isDragging = true;
    }

    void StopWindowDrag()
    {
        _isDragging = false;
    }

    public override void _Process(double delta)
    {
        if (!_isDragging) return;

        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            var window = GetWindow();
            var currentMousePosition = DisplayServer.MouseGetPosition();
            var mousePositionDelta = currentMousePosition - _dragStartMousePos;
            window.Position = _windowStartPos + mousePositionDelta;

            var screenSize = DisplayServer.ScreenGetSize();
            window.Position = new Vector2I(
                Mathf.Clamp(window.Position.X, 0, screenSize.X - 100),
                Mathf.Clamp(window.Position.Y, 0, screenSize.Y - 100)
            );
        }
        else
        {
            _stateMachine.Input_MouseReleased();
            StopWindowDrag();
        }
    }

    void OnDragAreaMouseEntered() => _stateMachine.Input_MouseEntered();

    void OnDragAreaMouseExited() => _stateMachine.Input_MouseExited();
    void OnDragAnimatorPickupEffectEnded() => _stateMachine.Input_PickupEffectEnded();
}