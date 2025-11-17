/*
* Oxygen.Flow library
* by karel66, 2023
*/

using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Oxygen
{
    /// <summary>
    /// Flow context with monadic Bind. 
    /// </summary>
    public readonly record struct Context
    {
        /// <summary>
        /// Selenium driver instance
        /// </summary>
        public readonly WebDriver Driver { get; }

        /// <summary>
        /// Remote web element
        /// </summary>
        public readonly WebElement Element { get; }

        /// <summary>
        /// Collection of elements
        /// </summary>
        public readonly ReadOnlyCollection<IWebElement> Collection { get; }


        public readonly object UserCredentials { get; }

        /// <summary>
        /// Run-time error result
        /// </summary>
        public readonly object Problem { get; }

        /// <summary>
        /// Generic constructor
        /// </summary>
        internal Context(WebDriver driver, WebElement element, ReadOnlyCollection<IWebElement> collection, object problem = null, object credentials = null)
        {
            Driver = driver;
            Element = element;
            Collection = collection;
            Problem = problem;
            UserCredentials = credentials;
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
        public readonly Context EmptyContext() =>
            new(this.Driver, null, null, null, null);

        /// <summary>
        /// Set context Element
        /// </summary>
        internal readonly Context NextContext(WebElement element) =>
            new(this.Driver, element, this.Collection, null, this.UserCredentials);

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
        internal readonly Context NextContext(ReadOnlyCollection<IWebElement> collection) =>
            new(this.Driver, this.Element, collection, this.Problem, this.UserCredentials);


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
        public readonly Context CreateProblem(object problem)
        {
            problem ??= "Null passed as problem!";

            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture)}Problem created: {problem}");

            return new Context(this.Driver, this.Element, this.Collection, problem, this.UserCredentials);
        }


        /// <summary>
        /// Monadic bind. 
        /// </summary>
        public Context Bind(FlowStep step)
        {
            if (this.HasProblem)
            {
                return this; // short-circuit problems
            }

            if (step == null)
            {
                return CreateProblem($"{nameof(Bind)}: NULL argument: {nameof(step)}");
            }

            string signature = $"{ExtractMethodName(step.Method.Name)} ({FormatTarget(step.Target)})";

            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture)} {signature}");

            try
            {
                return step.Invoke(this);
            }
            catch (UnhandledAlertException)
            {
                WebDriverWait wait = new(Driver, TimeSpan.FromSeconds(5));

                IAlert alert = wait.Until(drv =>
                {
                    try
                    {
                        return drv.SwitchTo().Alert();
                    }
                    catch (NoAlertPresentException)
                    {
                        return null;
                    }
                });

                alert?.Accept();

                return step.Invoke(this);
            }
            catch (Exception x)
            {
                return CreateProblem(x);
            }

        }

        static string FormatTarget(object target)
        {
            StringBuilder args = new();

            if (target != null)
            {
                Type type = target.GetType();

                foreach (System.Reflection.FieldInfo field in type.GetFields())
                {
                    object argval = field.GetValue(target);

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
                    else if (field.FieldType.IsValueType)
                    {
                        args.AppendWithComma($"{field.Name}={argval}");
                    }
                    else if (field.FieldType.BaseType != null && field.FieldType.BaseType.Name == "Array")
                    {
                        args.AppendWithComma($"{field.Name}=[");

                        foreach (object value in (Array)argval)
                        {
                            args.Append($"\"{value}\", ");
                        }

                        args.Append(']');
                    }

                    else if (field.FieldType.BaseType != null && field.FieldType.BaseType.Name == "MulticastDelegate")
                    {
                        args.AppendWithComma($"{field.Name}=[{field.FieldType.Name}]");
                    }
                    else
                    {
                        args.AppendWithComma($"{field.Name}={{");
                        foreach (string prop in field.FieldType.GetProperties().Select(prop => prop.Name))
                        {
                            try
                            {
                                args.Append($"{prop}:\"{field.FieldType.InvokeMember(prop, System.Reflection.BindingFlags.GetProperty, null, argval, null, null)}\", ");
                            }
                            catch (Exception x)
                            {
                                args.Append($"{prop}:\"<Exception of type {x.GetType().Name}>\", ");
                            }
                        }
                        args.Append('}');
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
            if (this.HasProblem)
            {
                return this; // short-circuit problems
            }

            if (action == null)
            {
                return CreateProblem($"{nameof(Use)}: NULL argument: {nameof(action)}");
            }

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

        public readonly bool TitleStartsWith(string title) =>
            new WebDriverWait(this.Driver, TimeSpan.FromSeconds(10))
                .Until(d => d.Title.StartsWith(title, StringComparison.InvariantCultureIgnoreCase));


        public override readonly string ToString()
        {
            if (HasProblem)
            {
                return Problem.ToString();
            }

            if (Driver != null)
            {
                return Driver.ToString();
            }

            return "Uninitialized Context";
        }

        public static implicit operator string(Context c) => c.ToString();

        /// <summary>
        /// Overloaded | operator for Context.Bind(FlowStep)
        /// </summary>
        public static Context operator |(Context a, FlowStep b) => a.Bind(b);

    }
}
