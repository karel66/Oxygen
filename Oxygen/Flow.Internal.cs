/*
* Oxygen.Flow library
* by karel66, 2023
*/

using System;
using System.Collections.ObjectModel;

using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;

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
        /// <param name="parent"></param>
        /// <param name="selector"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        static FlowStep ElementByCss(IFindsElement parent, string selector, int index = 0) => (Context context) =>
        {
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

        static bool ExistsByCss(IWebDriver driver, string selector, double seconds = 1.0)
        {
            try
            {
                WebDriverWait wait = new(driver, TimeSpan.FromSeconds(seconds));

                return wait.Until(drv => drv.FindElement(By.CssSelector(selector))) is WebElement result;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        static FlowStep ElementByXPath(IFindsElement parent, string xpath, int index = 0) => (Context context) =>
        {
            WebElement child = null;

            if (Retry(() =>
            {
                child = index == 0 ?
                    parent.FindElement(SeleniumFindMechanism.XPathSelectorMechanism, xpath) as WebElement
                    : parent.FindElements(SeleniumFindMechanism.XPathSelectorMechanism, xpath)[index] as WebElement;

                return child != null;
            }))
            {
                return context.NextContext(child);
            }

            return context.CreateProblem($"{nameof(ElementByXPath)}: '{xpath}' failed");
        };


        static FlowStep CollectionByXPath(IFindsElement parent, string xpath) => (Context context) =>
        {
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

        static bool ExistsByXPath(IWebDriver driver, string xpath, double seconds = 1.0)
        {
            try
            {
                WebDriverWait wait = new(driver, TimeSpan.FromSeconds(seconds));

                return wait.Until(drv => drv.FindElement(By.XPath(xpath))) is WebElement result;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }
    }
}
