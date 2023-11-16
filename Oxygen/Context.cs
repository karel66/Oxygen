/*
* Oxygen.Flow library
* by karel66, 2023
*/

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;

using OpenQA.Selenium;

namespace Elements.Oxygen
{
    /// <summary>
    /// Flow context with monadic Bind. 
    /// </summary>
    public struct Context
    {
        /// <summary>
        /// Selenium driver instance
        /// </summary>
        public WebDriver Driver { get; private set; }

        /// <summary>
        /// Remote web element
        /// </summary>
        public WebElement Element { get; private set; }

        /// <summary>
        /// Collection of elements
        /// </summary>
        public ReadOnlyCollection<IWebElement> Collection { get; private set; }

        /// <summary>
        /// Run-time error result
        /// </summary>
        public object Problem { get; private set; }

        string _taceIndent = "";

        /// <summary>
        /// Generic constructor
        /// </summary>
        internal Context(WebDriver driver, WebElement element, ReadOnlyCollection<IWebElement> collection, object problem = null)
        {
            Driver = driver;
            Element = element;
            Collection = collection;
            Problem = problem;
        }

        /// <summary>
        /// Indicates that there is a problem in the context.
        /// </summary>
        public readonly bool HasProblem => Problem != null;

        /// <summary>
        /// Indicates that there is an WebElement instance in the context.
        /// </summary>
        public readonly bool HasElement => Element != null;

        /// <summary>
        /// Returns current browser window title.
        /// </summary>
        public readonly string Title => Driver?.Title;

        /// <summary>
        /// Returns current context element text.
        /// </summary>
        public readonly string Text => Element?.Text;

        /// <summary>
        /// Returns current context element value attribute.
        /// </summary>
        public readonly string Value => Element?.GetAttribute("value");

        /// <summary>
        /// Returns context without Element or Collection.
        /// </summary>
        /// <returns></returns>
        public Context EmptyContext() =>
            new(this.Driver, null, null);

        /// <summary>
        /// Set context Element
        /// </summary>
        internal Context NextContext(WebElement element) =>
            new(this.Driver, element, this.Collection);

        /// <summary>
        /// Set context Element from generator
        /// </summary>
        internal Context NextContext(Func<WebElement> generator)
        {
            if (generator == null)
            {
                return CreateProblem($"{nameof(NextContext)}: NULL argument: {nameof(generator)}");
            }

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
        internal Context NextContext(ReadOnlyCollection<IWebElement> collection) =>
            new(this.Driver, this.Element, collection);


        /// <summary>
        /// Set context Collection from generator
        /// </summary>
        internal Context NextContext(Func<ReadOnlyCollection<IWebElement>> generator)
        {
            if (generator == null)
            {
                return CreateProblem($"{nameof(NextContext)}: NULL argument: {nameof(generator)}");
            }

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
        /// Set context Problem
        /// </summary>
        public Context CreateProblem(object problem)
        {
            problem ??= "Null passed as problem!";

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

            var signature = $"{ExtractMethodName(step.Method.Name)} ({FormatTarget(step.Target)})";

            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture)} {_taceIndent}{signature}");

            _taceIndent += "  ";

            try
            {
                return step.Invoke(this);
            }
            catch (Exception x)
            {
                return CreateProblem(x);
            }
            finally
            {
                if (_taceIndent.Length > 1)
                {
                    _taceIndent = _taceIndent.Remove(_taceIndent.Length - 2);
                }
            }
        }

        static string FormatTarget(object target)
        {
            StringBuilder args = new();

            if (target != null)
            {
                var type = target.GetType();

                foreach (var field in type.GetFields())
                {
                    var argval = field.GetValue(target);

                    if (argval == null)
                    {
                        args.AppendWithComma($"{field.Name}=null");
                    }
                    else if (field.FieldType.Name == "String")
                    {
                        args.AppendWithComma($"{field.Name}=\"{argval}\"");
                    }
                    else if (field.FieldType.Name == "Char")
                    {
                        args.AppendWithComma($"{field.Name}='{argval}'");
                    }
                    else if (field.FieldType.BaseType != null && field.FieldType.BaseType.Name == "MulticastDelegate")
                    {
                        var deleg = argval as MulticastDelegate;

                        var method = deleg.Method;

                        args.AppendWithComma($"{field.Name}={ExtractMethodName(method.Name)}");

                        if (deleg.Target != target)
                        {
                            args.Append(System.Globalization.CultureInfo.InvariantCulture, $" ({FormatTarget(deleg.Target)})");
                        }
                    }
                    else
                    {
                        foreach (var prop in field.FieldType.GetProperties().Select(prop => prop.Name))
                        {
                            try
                            {
                                args.AppendWithComma($" [{prop}={field.FieldType.InvokeMember(prop, BindingFlags.GetProperty, null, argval, null, null)}]");
                            }
                            catch (Exception x)
                            {
                                args.AppendWithComma($" [{prop}=<Exception of type {x.GetType().Name}>]");
                            }
                        }
                    }
                }
            }
            return args.ToString();
        }

        static string ExtractMethodName(string reflectedName)
        {
            string name = reflectedName;
            if (name[0] == '<')
            {
                name = name[1..name.IndexOf('>')];
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

        public bool TitleStartsWith(string title)
        {
            while (string.IsNullOrEmpty(this.Title))
            {
                System.Threading.Thread.Sleep(100);
            }

            for (int i = 0; i < 10 && !this.Title.StartsWith(title, StringComparison.InvariantCultureIgnoreCase); i++)
            {
                System.Threading.Thread.Sleep(100);
            }

            return this.Title.StartsWith(title, StringComparison.InvariantCultureIgnoreCase);

        }

        public override string ToString()
        {
            if (HasProblem) return Problem.ToString();
            if (Driver != null) return Driver.ToString();
            return "Uninitialized Context";
        }

        public static implicit operator string(Context c) => c.ToString();

        /// <summary>
        /// Overloaded | operator for Context.Bind(FlowStep)
        /// </summary>
        public static Context operator |(Context a, FlowStep b) => a.Bind(b);

    }
}
