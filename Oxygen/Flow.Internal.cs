/*
Oxygen Flow library
*/

using System;
using System.Collections.ObjectModel;

using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;

namespace Oxygen
{
    public partial class Flow
    {

        static FlowStep ElementByCss(IFindsByCssSelector parent, string cssSelector, int index = 0) => (Context context) =>
        {
            O($"Element: {cssSelector} [{index}]");

            RemoteWebElement child = null;

            if (!TryUntilSuccess(() =>
            {
                child = index == 0 ?
                    parent.FindElementByCssSelector(cssSelector) as RemoteWebElement
                    : parent.FindElementsByCssSelector(cssSelector)[index] as RemoteWebElement;

                return child != null;
            }))
            {
                return context.CreateProblem($"{nameof(ElementByCss)}: '{cssSelector}' failed");
            }

            return context.NextContext(child);
        };


        static FlowStep CollectionByCss(IFindsByCssSelector parent, string cssSelector) => (Context context) =>
        {
            O($"Collection: {cssSelector}");

            ReadOnlyCollection<IWebElement> result = null;

            if (!TryUntilSuccess(() =>
            {
                result = parent.FindElementsByCssSelector(cssSelector);

                return result != null && result.Count > 0; // Satisfied only by non-empty collection
            }))
            {
                return context.CreateProblem($"{nameof(CollectionByCss)}: '{cssSelector}' failed");
            }

            return context.NextContext(result);
        };

        static bool ExistsByCss(IFindsByCssSelector parent, string selector) => ElementExists(parent.FindElementByCssSelector, selector);

        static bool ElementExists(Func<string, IWebElement> find, string filter)
        {
            try
            {
                var result = find(filter) as RemoteWebElement;

                O($"Exists: {filter} : {result != null}");

                return result != null;
            }
            catch (NoSuchElementException)
            {
                O($"Exists: {filter} : NO");
                return false;
            }
        }

        static FlowStep ElementByXPath(IFindsByXPath parent, string xpath, int index = 0) => (Context context) =>
        {
            O($"{nameof(ElementByXPath)}: {xpath} [{index}]");
            RemoteWebElement child = null;

            if (!TryUntilSuccess(() =>
            {
                child = index == 0 ?
                    parent.FindElementByXPath(xpath) as RemoteWebElement
                    : parent.FindElementsByXPath(xpath)[index] as RemoteWebElement;

                return child != null;
            }))
            {
                return context.CreateProblem($"{nameof(ElementByXPath)}: '{xpath}' failed");
            }

            return context.NextContext(child);
        };


        static FlowStep CollectionByXPath(IFindsByXPath parent, string xpath) => (Context context) =>
        {
            O($"CollectionByXPath: {xpath}");

            ReadOnlyCollection<IWebElement> result = null;

            if (!TryUntilSuccess(() =>
            {
                result = parent.FindElementsByXPath(xpath);

                return result != null && result.Count > 0; // Satisfied only by non-empty collection
            }))
            {
                return context.CreateProblem($"{nameof(CollectionByXPath)}: '{xpath}' failed");
            }

            return context.NextContext(result);
        };

        static bool ExistsByXPath(IFindsByXPath parent, string xpath) => ElementExists(parent.FindElementByXPath, xpath);
    }
}
