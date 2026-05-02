using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace AnalogClock.DragMechanic;

[Meta, LogicBlock(typeof(State), Diagram = true)]
public partial class DragStateMachine : LogicBlock<DragStateMachine.State>
{
    public static class Input
    {
        public readonly record struct MouseEntered;
        public readonly record struct MouseExited;
        public readonly record struct LeftMousePressed;
        public readonly record struct LeftMouseReleased;
        public readonly record struct PickupEffectEnded;
    }

    public static class Output
    {
        public readonly record struct EnteredIdle;
        public readonly record struct EnteredHover;
        public readonly record struct EnteredPickup;
        public readonly record struct EnteredHolding;
        public readonly record struct EnteredDrop;
    }

    public override Transition GetInitialState() => To<State.Idle>();

    public abstract record State : StateLogic<State>
    {
        public abstract record IdleBase : State;

        public record Idle : IdleBase, IGet<Input.MouseEntered>, IGet<Input.LeftMousePressed>
        {
            public Idle()
            {
                OnAttach(() => Output(new Output.EnteredIdle()));
            }
            
            public Transition On(in Input.MouseEntered input) => To<Hover>();
            public Transition On(in Input.LeftMousePressed input) => To<Pickup>();
        }

        public record Hover : IdleBase, IGet<Input.MouseExited>, IGet<Input.LeftMousePressed>
        {
            public Hover()
            {
                OnAttach(() => Output(new Output.EnteredHover()));
            }

            public Transition On(in Input.MouseExited input) => To<Idle>();
            public Transition On(in Input.LeftMousePressed input) => To<Pickup>();
        }


        public abstract record DraggingBase : State;

        public record Pickup : DraggingBase, IGet<Input.LeftMouseReleased>, IGet<Input.PickupEffectEnded>
        {
            public Pickup()
            {
                OnAttach(() => Output(new Output.EnteredPickup()));
            }
            
            public Transition On(in Input.LeftMouseReleased input) => To<Idle>();
            public Transition On(in Input.PickupEffectEnded input) => To<Holding>();
        }

        public record Holding : DraggingBase, IGet<Input.LeftMouseReleased>
        {
            public Holding()
            {
                OnAttach(() =>
                {
                    Output(new Output.EnteredHolding());
                });
            }

            public Transition On(in Input.LeftMouseReleased input) => To<Idle>();
        }
    }

    public void Input_MouseEntered() => Input(new Input.MouseEntered());
    public void Input_MouseExited() => Input(new Input.MouseExited());
    public void Input_MousePressed() => Input(new Input.LeftMousePressed());
    public void Input_MouseReleased() => Input(new Input.LeftMouseReleased());
    public void Input_PickupEffectEnded() => Input(new Input.PickupEffectEnded());
}