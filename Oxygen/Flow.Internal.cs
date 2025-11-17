/*
* Oxygen.Flow library
* by karel66, 2023
*/

using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.ObjectModel;

namespace Oxygen
{
    /// <summary>
    /// Internal methods
    /// </summary>
    public partial class Flow
    {
        /// <summary>
        /// Returns first element if index=0, last element if index=-1
        /// </summary>
        static FlowStep ElementByCss(IFindsElement parent, string selector, int index = 0) => (Context context) =>
        {
            if (parent == null)
            {
                return context.CreateProblem($"{nameof(ElementByCss)}: parent is null for selector '{selector}'");
            }

            ReadOnlyCollection<IWebElement> result = null;
            WebElement child = null;

            if (Retry(() =>
            {
                result = parent.FindElements(SeleniumFindMechanism.CssSelectorMechanism, selector);
                if (result.Count > 0)
                {
                    if (index == -1)
                    {
                        child = result[^1] as WebElement;
                    }
                    else if (index < result.Count)
                    {
                        child = result[index] as WebElement;
                    }
                }

                return child != null;
            }))
            {
                return context.NextContext(child);
            }

            return context.CreateProblem($"{nameof(ElementByCss)}: '{selector}' failed");
        };


        static FlowStep CollectionByCss(IFindsElement parent, string selector) => (Context context) =>
        {
            if (parent == null)
            {
                return context.CreateProblem($"{nameof(CollectionByCss)}: parent is null for selector '{selector}'");
            }

            ReadOnlyCollection<IWebElement> result = null;

            if (Retry(() =>
            {
                result = parent.FindElements(SeleniumFindMechanism.CssSelectorMechanism, selector);

                return result != null && result.Count > 0; // Satisfied only by non-empty collection
            }))
            {
                return context.NextContext(result);
            }

            return context.CreateProblem($"{nameof(CollectionByCss)}: '{selector}' failed");

        };

        public static bool ExistsByCss(IWebDriver driver, string selector, double seconds = 1.0)
        {
            try
            {
                WebDriverWait wait = new(driver, TimeSpan.FromSeconds(seconds));

                wait.Until(drv => drv.FindElement(By.CssSelector(selector)));

                return true;
            }
            catch (NoSuchElementException)
            {
                Log($"ExistsByCss [{selector}] not found");
                return false;
            }
            catch (WebDriverTimeoutException)
            {
                Log($"ExistsByCss [{selector}] not found");
                return false;
            }
        }

        static FlowStep ElementByXPath(IFindsElement parent, string xpath, int index = 0) => (Context context) =>
        {
            if (parent == null)
            {
                return context.CreateProblem($"{nameof(ElementByXPath)}: parent is null for selector '{xpath}'");
            }

            WebElement child = index == 0 ?
                parent.FindElement(SeleniumFindMechanism.XPathSelectorMechanism, xpath) as WebElement
                    : parent.FindElements(SeleniumFindMechanism.XPathSelectorMechanism, xpath)[index] as WebElement;

            if (child != null)
            {
                return context.NextContext(child);
            }

            return context.CreateProblem($"{nameof(ElementByXPath)}: '{xpath}' failed");
        };


        static FlowStep CollectionByXPath(IFindsElement parent, string xpath) => (Context context) =>
        {
            if (parent == null)
            {
                return context.CreateProblem($"{nameof(CollectionByXPath)}: parent is null for selector '{xpath}'");
            }

            ReadOnlyCollection<IWebElement> result = null;

            if (Retry(() =>
            {
                result = parent.FindElements(SeleniumFindMechanism.XPathSelectorMechanism, xpath);

                return result != null && result.Count > 0; // Satisfied only by non-empty collection
            }))
            {
                return context.NextContext(result);
            }

            return context.CreateProblem($"{nameof(CollectionByXPath)}: '{xpath}' failed");
        };

        public static bool ExistsByXPath(IWebDriver driver, string xpath, double seconds = 1.0)
        {
            try
            {
                WebDriverWait wait = new(driver, TimeSpan.FromSeconds(seconds));

                return wait.Until(drv => drv.FindElement(By.XPath(xpath))) is WebElement result;
            }
            catch (NoSuchElementException)
            {
                Log($"ExistsByXPath [{xpath}] not found");
                return false;
            }
            catch (WebDriverTimeoutException)
            {
                Log($"ExistsByXPath [{xpath}] not found");
                return false;
            }
        }
    }
}
