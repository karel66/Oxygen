/*
 * Oxygen.Flow library
 * by karel66, 2020
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
    public struct Context
    {
        /// <summary>
        /// Selenium driver instance
        /// </summary>
        public RemoteWebDriver Driver { get; private set; }

        /// <summary>
        /// Remote web element
        /// </summary>
        public RemoteWebElement Element { get; private set; }

        /// <summary>
        /// Collection of elements
        /// </summary>
        public ReadOnlyCollection<IWebElement> Collection { get; private set; }

        /// <summary>
        /// Run-time error result
        /// </summary>
        public object Problem { get; private set; }

        static string TaceIndent = "";
        /// <summary>
        /// Generic constructor
        /// </summary>
        internal Context(RemoteWebDriver driver, RemoteWebElement element, ReadOnlyCollection<IWebElement> collection, object problem = null)
        {
            Driver = driver;
            Element = element;
            Collection = collection;
            Problem = problem;
        }

        /// <summary>
        /// Indicates that there is a problem in the context.
        /// </summary>
        public bool HasProblem => Problem != null;

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
        internal Context NextContext(RemoteWebElement element) => new Context(this.Driver, element, this.Collection);

        /// <summary>
        /// Set context Element from generator
        /// </summary>
        internal Context NextContext(Func<RemoteWebElement> generator)
        {
            if (generator == null) return CreateProblem($"{nameof(NextContext)}: NULL argument: {nameof(generator)}");

            try
            {
                return NextContext(generator.Invoke());
            }
            catch (Exception x)
            {
                return CreateProblem(x);
            }
        }

        /// <summary>
        /// Set context Collection
        /// </summary>
        internal Context NextContext(ReadOnlyCollection<IWebElement> collection) => new Context(this.Driver, this.Element, collection);


        /// <summary>
        /// Set context Collection from generator
        /// </summary>
        internal Context NextContext(Func<ReadOnlyCollection<IWebElement>> generator)
        {
            if (generator == null) return CreateProblem($"{nameof(NextContext)}: NULL argument: {nameof(generator)}");

            try
            {
                return new Context(this.Driver, this.Element, generator.Invoke());
            }
            catch (Exception x)
            {
                return CreateProblem(x);
            }
        }


        /// <summary>
        /// Set context Problem
        /// </summary>
        public Context CreateProblem(object problem)
        {
            if (problem == null) problem = "Null passed as problem!";

            Console.WriteLine(problem.ToString());

            return new Context(this.Driver, this.Element, this.Collection, problem);
        }


        /// <summary>
        /// Monadic bind. 
        /// </summary>
        public Context Bind(FlowStep step)
        {
            if (this.HasProblem) return this; // short-circuit problems

            if (step == null) return CreateProblem($"{nameof(Bind)}: NULL argument: {nameof(step)}");

            var signature = $"{ExtractMethodName(step.Method.Name)} {FormatTarget(step.Target)}";
            try
            {
                Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture)} {TaceIndent}{signature}");
                TaceIndent += '\t';

                return step.Invoke(this);
            }
            catch (Exception x)
            {
                return CreateProblem(x);
            }
            finally
            {
                if (TaceIndent.Length > 0) { TaceIndent = TaceIndent.Remove(TaceIndent.Length - 1, 1); }
            }
        }

        static string FormatTarget(object target)
        {
            string args = null;

            if (target != null)
            {
                var type = target.GetType();

                foreach (var field in type.GetFields())
                {
                    var argval = type.InvokeMember(field.Name, System.Reflection.BindingFlags.GetField, null, target, null, null);

                    if (args != null) args += ", ";

                    if (field.FieldType.Name == "String")
                    {
                        args += $"\"{argval}\"";
                    }
                    else if (field.FieldType.Name == "Char")
                    {
                        args += $"'{argval}'";
                    }
                    else if (field.FieldType.BaseType.Name == "MulticastDelegate")
                    {
                        var deleg = argval as MulticastDelegate;
                        var method = deleg.Method;

                        args += $"{field.Name}=>{method.DeclaringType.Name}.{ExtractMethodName(method.Name)}";
                        if (deleg.Target != target)
                        {
                            args += $"({FormatTarget(deleg.Target)})";
                        }
                    }
                    else
                    {
                        args += $"{argval}";
                    }
                }
            }
            return args;
        }

        static string ExtractMethodName(string reflectedName)
        {
            string name = reflectedName;
            if (name[0] == '<')
            {
                name = name.Substring(1, name.IndexOf('>') - 1);
            }
            return name;
        }

        /// <summary>
        /// Action bind.
        /// </summary>
        public Context Use(Action<Context> action)
        {
            if (this.HasProblem) return this; // short-circuit problems

            if (action == null) return CreateProblem($"{nameof(Use)}: NULL argument: {nameof(action)}");

            try
            {
                action.Invoke(this);
                return this;
            }
            catch (Exception x)
            {
                return CreateProblem(x);
            }
        }

        public override string ToString() => HasProblem ? Problem.ToString() : Driver != null ? Driver.ToString() : "Uninitialized Context";

        public static implicit operator string(Context c) => c.ToString();

        /// <summary>
        /// Overloaded | operator for Context.Bind(FlowStep)
        /// </summary>
        public static Context operator |(Context a, FlowStep b) => a.Bind(b);

    }
}
