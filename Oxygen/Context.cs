/*
Oxygen Flow library
*/

using System;
using System.Collections.ObjectModel;

using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Oxygen
{
    /// <summary>
    /// Flow context with monadic Bind. 
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "<Pending>")]
    public struct Context
    {
        /// <summary>
        /// Selenium driver instance
        /// </summary>
        public RemoteWebDriver Driver { get; private set; }

        /// <summary>
        /// Remote web element retrieved by flow step
        /// </summary>
        public RemoteWebElement Element { get; private set; }

        /// <summary>
        /// Collection of elements retrieved by flow step
        /// </summary>
        public ReadOnlyCollection<IWebElement> Collection { get; private set; }

        /// <summary>
        /// Problem cause
        /// </summary>
        public object ProblemCause { get; private set; }

        /// <summary>
        /// Generic constructor
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="element"></param>
        /// <param name="collection"></param>
        /// <param name="problemCause"></param>
        internal Context(RemoteWebDriver driver, RemoteWebElement element, ReadOnlyCollection<IWebElement> collection, object problemCause = null)
        {
            Driver = driver;
            Element = element;
            Collection = collection;
            ProblemCause = problemCause;
        }

        /// <summary>
        /// Indicates that there is a problem in the context.
        /// </summary>
        public bool HasProblem => ProblemCause != null;

        /// <summary>
        /// Indicates that there is an RemoteWebElement instance in the context.
        /// </summary>
        public bool HasElement => Element != null;

        /// <summary>
        /// Returns current browser window title.
        /// </summary>
        public string Title => Driver?.Title;

        /// <summary>
        /// Returns current context element text.
        /// </summary>
        public string Text => Element?.Text;

        /// <summary>
        /// Returns current context element value attribute.
        /// </summary>
        public string Value => Element?.GetAttribute("value");

        /// <summary>
        /// Set context Element
        /// </summary>
        internal Context NewContext(RemoteWebElement element) => new Context(this.Driver, element, this.Collection);


        /// <summary>
        /// Set context Element from generator
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        internal Context NewContext(Func<RemoteWebElement> generator)
        {
            if (generator == null) return NewProblem($"{nameof(NewContext)}: NULL argument: {nameof(generator)}");

            try
            {
                var element = generator();

                if (element == null) return NewProblem($"{nameof(NewContext)}: {nameof(generator)} produced NULL element.");

                return NewContext(element);
            }
            catch (Exception x)
            {
                return NewProblem(x);
            }
        }

        /// <summary>
        /// Set context Collection
        /// </summary>
        internal Context NewContext(ReadOnlyCollection<IWebElement> collection) => new Context(this.Driver, this.Element, collection);


        /// <summary>
        /// Set context Collection from generator
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        internal Context NewContext(Func<ReadOnlyCollection<IWebElement>> generator)
        {
            if (generator == null) return NewProblem($"{nameof(NewContext)}: NULL argument: {nameof(generator)}");

            try
            {
                var collection = generator();

                if (collection == null) return NewProblem($"{nameof(NewContext)}: {nameof(generator)} produced NULL collection.");

                return new Context(this.Driver, this.Element, collection);
            }
            catch (Exception x)
            {
                return NewProblem(x);
            }
        }


        /// <summary>
        /// Set context Problem
        /// </summary>
        public Context NewProblem(object problemCause)
        {
            if (problemCause == null)
            {
                problemCause = "Null passed as problem cause!";
            }

            System.Diagnostics.Trace.TraceError(problemCause.ToString());

            return new Context(this.Driver, this.Element, this.Collection, problemCause);
        }



        /// <summary>
        /// Monadic bind. 
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public Context Bind(FlowStep step)
        {
            if (this.HasProblem) return this; // short-circuit problems

            if (step == null) return NewProblem($"{nameof(Bind)}: NULL argument: {nameof(step)}");

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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public Context Use(Action<Context> action)
        {
            if (HasProblem) return this; // short-circuit problems

            if (action == null) return NewProblem($"{nameof(Use)}: NULL argument: {nameof(action)}");

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
          HasProblem ? ProblemCause.ToString() : Driver != null ? Driver.ToString() : "Uninitialized Context";

        public static implicit operator string(Context c) => c.ToString();


        /// <summary>
        /// Overloaded | operator for Context.Bind(FlowStep)
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "<Pending>")]
        public static Context operator |(Context a, FlowStep b) => a.Bind(b);

    }
}
