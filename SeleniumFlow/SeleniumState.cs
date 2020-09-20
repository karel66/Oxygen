/*
Oxygen Flow library
*/

using System;
using System.Collections.ObjectModel;

using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Oxygen;

namespace SeleniumFlow
{
    /// <summary>
    /// Flow context with monadic Bind. 
    /// </summary>
    public struct SeleniumState
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


        public SeleniumState(RemoteWebDriver driver, RemoteWebElement element, ReadOnlyCollection<IWebElement> collection)
        {
            Driver = driver;
            Element = element;
            Collection = collection;
        }

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
        /// cretaes new context from the driver instance
        /// </summary>
        /// <param name="driver"></param>
        /// <returns></returns>
        public static SeleniumState FromDriver(RemoteWebDriver driver) => new SeleniumState(driver, null, null);

        /// <summary>
        /// Set context Element
        /// </summary>
        public SeleniumState FromElement(RemoteWebElement element) =>
            new SeleniumState(Driver, element, Collection);

        /// <summary>
        /// Set context Element from generator
        /// </summary>
        public SeleniumState FromElement(Func<RemoteWebElement> generator)
        {
            if (generator == null) throw new Exception("Missing collection generator.");
            return new SeleniumState(Driver, generator(), Collection);
        }

        /// <summary>
        /// Set context Collection
        /// </summary>
        public SeleniumState FromCollection(ReadOnlyCollection<IWebElement> collection) =>
            new SeleniumState(Driver, Element, collection);

        /// <summary>
        /// Set context Collection from generator
        /// </summary>
        public SeleniumState FromCollection(Func<ReadOnlyCollection<IWebElement>> generator)
        {
            if (generator == null) throw new Exception("Missing collection generator.");
            return new SeleniumState(Driver, Element, generator());
        }
    }

}
