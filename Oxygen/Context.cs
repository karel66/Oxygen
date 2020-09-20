/*
Oxygen Flow library
*/

using System;

namespace Oxygen
{
    namespace Flow
    {
        public delegate Context<TState> Step<TState>(Context<TState> context);

        public struct Context<TState>
        {
            /// <summary>
            /// Current state
            /// </summary>
            public TState State { get; private set; }

            /// <summary>
            /// Current problem
            /// </summary>
            public object Problem { get; private set; }

            Context(TState state, object problem)
            {
                State = state;
                Problem = problem;

                if (state == null && problem == null)
                {
                    Problem = "All null arguments in contructor.";
                }
            }

            public static Context<TState> NewState(TState state) => new Context<TState>(state, null);

            /// <summary>
            /// Set context Problem
            /// </summary>
            public Context<TState> NewProblem(object problem)
            {
                if (problem == null)
                {
                    problem = "Null as problem cause argument.";
                }

                System.Diagnostics.Trace.TraceError(problem.ToString());

                return new Context<TState>(State, problem);
            }


            /// <summary>
            /// Monadic bind. 
            /// </summary>
            public Context<TState> Bind(Step<TState> step)
            {
                if (step == null) return NewProblem("Null argument in Bind(step).");

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
            /// Action bind.
            /// </summary>
            public Context<TState> Use(Action<Context<TState>> action)
            {
                if (action == null) return NewProblem("NULL argument in Bind(action).");

                if (Problem != null) return this; // short-circuit problems

                try
                {
                    action(this);
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
            public static Context<TState> operator |(Context<TState> a, Step<TState> b) => a.Bind(b);
        }

    }
}
