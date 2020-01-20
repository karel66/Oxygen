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

            string errorMsg = TryAndWait10Times(() =>
            {
                child = index == 0 ?
                    parent.FindElementByCssSelector(cssSelector) as RemoteWebElement
                    : parent.FindElementsByCssSelector(cssSelector)[index] as RemoteWebElement;

                return child != null;
            });

            if (!string.IsNullOrEmpty(errorMsg) || child == null)
            {
                LogError(errorMsg, null);
                return context.Problem(errorMsg);
            }

            return context.FromElement(child);
        };


        static FlowStep CollectionByCss(IFindsByCssSelector parent, string cssSelector) => (Context context) =>
        {
            O($"Collection: {cssSelector}");

            ReadOnlyCollection<IWebElement> result = null;

            string errorMsg = TryAndWait10Times(() =>
            {
                result = parent.FindElementsByCssSelector(cssSelector);

                return result != null && result.Count > 0; // Satisfied only by non-empty collection
            });

            if (!string.IsNullOrEmpty(errorMsg) || result == null || result.Count == 0)
            {
                LogError(errorMsg, null);
                return context.Problem(errorMsg);
            }

            return context.FromCollection(result);
        };

        static bool ExistsByCss(IFindsByCssSelector parent, string selector) => ElementExists(parent.FindElementByCssSelector, selector);

        static bool ElementExists(Func<string, IWebElement> find, string filter)
        {
            try
            {
                return (find(filter) as RemoteWebElement) != null;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        static FlowStep ElementByXPath(IFindsByXPath parent, string xpath, int index = 0) => (Context context) =>
        {
            O($"ElementByXPath: {xpath} [{index}]");
            RemoteWebElement child = null;

            string errorMsg = TryAndWait10Times(() =>
            {
                child = index == 0 ?
                    parent.FindElementByXPath(xpath) as RemoteWebElement
                    : parent.FindElementsByXPath(xpath)[index] as RemoteWebElement;

                return child != null;
            });

            if (!string.IsNullOrEmpty(errorMsg) || child == null)
            {
                LogError(errorMsg, null);
                return context.Problem(errorMsg);
            }

            return context.FromElement(child);
        };


        static FlowStep CollectionByXPath(IFindsByXPath parent, string xpath) => (Context context) =>
        {
            O($"CollectionByXPath: {xpath}");

            ReadOnlyCollection<IWebElement> result = null;

            string errorMsg = TryAndWait10Times(() =>
            {
                result = parent.FindElementsByXPath(xpath);

                return result != null && result.Count > 0; // Satisfied only by non-empty collection
            });

            if (!string.IsNullOrEmpty(errorMsg) || result == null || result.Count == 0)
            {
                LogError(errorMsg, null);
                return context.Problem(errorMsg);
            }

            return context.FromCollection(result);
        };

        static bool ExistsByXPath(IFindsByXPath parent, string xpath) => ElementExists(parent.FindElementByXPath, xpath);
    }
}
