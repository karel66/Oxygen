/*** Oxygen Flow for Selenium ***/

using System;
using System.Collections.ObjectModel;

using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Oxygen
{
    /// <summary>
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

        public bool HasElement => Element != null;

        public string Title => Driver?.Title;

        public string Text => Element?.Text;

        public string Value => Element?.GetAttribute("value");

        public static SeleniumState FromDriver(RemoteWebDriver driver) => new SeleniumState(driver, null, null);

        public SeleniumState FromElement(RemoteWebElement element) =>
            new SeleniumState(this.Driver, element, this.Collection);

        public SeleniumState FromElement(Func<RemoteWebElement> generator)
        {
            if (generator == null)
            {
                throw new ArgumentNullException("Missing collection generator.");
            }

            return new SeleniumState(this.Driver, generator(), this.Collection);
        }

        public SeleniumState FromCollection(ReadOnlyCollection<IWebElement> collection) =>
            new SeleniumState(this.Driver, this.Element, collection);

        public SeleniumState FromCollection(Func<ReadOnlyCollection<IWebElement>> generator)
        {
            if (generator == null)
            {
                throw new ArgumentNullException("Missing collection generator.");
            }

            return new SeleniumState(this.Driver, this.Element, generator());
        }
    }
}