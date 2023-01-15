﻿/*
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
        /// <param name="cssSelector"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        static FlowStep ElementByCss(IFindsElement parent, string cssSelector, int index = 0) => (Context context) =>
        {
            ReadOnlyCollection<IWebElement> result = null;
            WebElement child = null;

            if (!TryUntilSuccess(() =>
            {
                result = parent.FindElements(SeleniumFindMechanism.CssSelectorMechanism, cssSelector);
                if (result.Count > 0)
                {
                    if (index == -1)
                    {
                        child = result[result.Count - 1] as WebElement;
                    }
                    else if (index < result.Count)
                    {
                        child = result[index] as WebElement;
                    }
                }

                return child != null;
            }))
            {
                return context.CreateProblem($"{nameof(ElementByCss)}: '{cssSelector}' failed");
            }

            return context.NextContext(child);
        };


        static FlowStep CollectionByCss(IFindsElement parent, string cssSelector) => (Context context) =>
        {
            ReadOnlyCollection<IWebElement> result = null;

            if (!TryUntilSuccess(() =>
            {
                result = parent.FindElements(SeleniumFindMechanism.CssSelectorMechanism, cssSelector);

                return result != null && result.Count > 0; // Satisfied only by non-empty collection
            }))
            {
                return context.CreateProblem($"{nameof(CollectionByCss)}: '{cssSelector}' failed");
            }

            return context.NextContext(result);
        };

        static bool ExistsByCss(IWebDriver driver, string selector, double seconds = 1.0)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(seconds));

                WebElement result = wait.Until(drv => drv.FindElement(By.CssSelector(selector))) as WebElement;

                return result != null;
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

            if (!TryUntilSuccess(() =>
            {
                child = index == 0 ?
                    parent.FindElement(SeleniumFindMechanism.XPathSelectorMechanism, xpath) as WebElement
                    : parent.FindElements(SeleniumFindMechanism.XPathSelectorMechanism, xpath)[index] as WebElement;

                return child != null;
            }))
            {
                return context.CreateProblem($"{nameof(ElementByXPath)}: '{xpath}' failed");
            }

            return context.NextContext(child);
        };


        static FlowStep CollectionByXPath(IFindsElement parent, string xpath) => (Context context) =>
        {
            ReadOnlyCollection<IWebElement> result = null;

            if (!TryUntilSuccess(() =>
            {
                result = parent.FindElements(SeleniumFindMechanism.XPathSelectorMechanism, xpath);

                return result != null && result.Count > 0; // Satisfied only by non-empty collection
            }))
            {
                return context.CreateProblem($"{nameof(CollectionByXPath)}: '{xpath}' failed");
            }

            return context.NextContext(result);
        };

        static bool ExistsByXPath(IWebDriver driver, string xpath, double seconds = 1.0)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(seconds));

                WebElement result = wait.Until(drv => drv.FindElement(By.XPath(xpath))) as WebElement;

                return result != null;
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
