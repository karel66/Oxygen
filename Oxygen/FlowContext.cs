
/*** Oxygen Flow ***/

using System;

namespace Oxygen
{
    public delegate FlowContext<TState> FlowStep<TState>(FlowContext<TState> context);

    public struct FlowContext<TState>
    {
        /// <summary>
        /// Current state
        /// </summary>
        public TState State { get; private set; }

        /// <summary>
        /// Current problem
        /// </summary>
        public object Problem { get; private set; }

        public FlowContext(TState state, object problem)
        {
            State = state;
            Problem = problem;

            if (state == null && problem == null)
            {
                Problem = $"{nameof(FlowContext<TState>)}: NULL arguments.";
            }
        }

        /// <summary>
        /// Create new context from the given problem
        /// </summary>
        public FlowContext<TState> NewProblem(object problem)
        {
            if (problem == null)
            {
                problem = $"{nameof(NewProblem)}: NULL argument.";
            }

            System.Diagnostics.Trace.TraceError(problem.ToString());

            return new FlowContext<TState>(this.State, problem);
        }

        public FlowContext<TState> NewState(TState state)
        {
            if (state == null)
            {
                return NewProblem($"{nameof(NewState)}: NULL argument.");
            }

            return new FlowContext<TState>(state, null);
        }


        /// <summary>
        /// Monadic bind - executes the step on current state which returns a new context.
        /// </summary>
        public FlowContext<TState> Bind(FlowStep<TState> step)
        {
            if (step == null) return NewProblem($"{nameof(Bind)}: NULL argument).");

            if (Problem != null) return this; // short-circuit problems

            try
            {
                return step(this);
            }
            catch (Exception x)
            {
                return NewProblem(x);
            }
        }

        /// <summary>
        /// Executes the action on current state and returns the current context.
        /// </summary>
        /// <remarks>Action should not mutate the State.</remarks>
        public FlowContext<TState> Use(Action<TState> action)
        {
            if (action == null) return NewProblem($"{nameof(Use)}: NULL argument).");

            if (Problem != null) return this; // short-circuit problems

            try
            {
                action(State);
                return this;
            }
            catch (Exception x)
            {
                return NewProblem(x);
            }
        }

        public override string ToString() =>
          Problem != null ? Problem.ToString() : State != null ? State.ToString() : "Uninitialized Context";

        /// <summary>
        /// Overloaded | operator for bind.
        /// </summary>
        public static FlowContext<TState> operator |(FlowContext<TState> a, FlowStep<TState> b) => a.Bind(b);

    }
}
