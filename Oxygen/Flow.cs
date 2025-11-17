/*
* Oxygen.Flow library
* by karel66, 2023
*/

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using System;
using System.Diagnostics;

namespace Oxygen
{
    /// <summary>
    /// Base class for Selenium UI tests.
    /// </summary>
    public partial class Flow
    {
        protected Flow() { }

        /// <summary>
        /// Instantiates selected browser.
        /// </summary>
        /// <param name="browserBrand">Browser brand to instantiate.</param>
        /// <param name="startPageUrl">Start page URL.</param
        /// <param name="implicitWait">Selenium driver implicit wait duration, in seconds.</param>
        /// <param name="killOthers">Optional. If true then other browser instances are closed.</param>
        /// <param name="driverDirectory">Optional driver directory. If not spcefied then environment PATH is used.</param>
        /// <param name="options">Specific driver options. Must match the browser brand options type.</param>
        /// <returns>Driver in context</returns>
        public static Context CreateContext(BrowserBrand browserBrand, Uri startPageUrl, double implicitWait = 1.0, bool killOthers = false, string driverDirectory = null, DriverOptions options = null)
        {
            if (startPageUrl == null) { throw new ArgumentException($"{nameof(CreateContext)}: NULL argument: {nameof(startPageUrl)}"); }

            WebDriver driver = null;

            switch (browserBrand)
            {
                case BrowserBrand.Chrome:
                    {
                        if (killOthers)
                        {
                            KillBrowserProcesses("chrome");
                        }

                        driver = InitChromeDriver(driverDirectory, options);
                    }
                    break;

                case BrowserBrand.Edge:
                    {
                        if (killOthers)
                        {
                            KillBrowserProcesses("MicrosoftEdge", "MicrosoftWebDriver", "msedgedriver");
                        }

                        driver = InitEdgeDriver(driverDirectory, options);
                    }
                    break;

                case BrowserBrand.FireFox:
                    {
                        if (killOthers)
                        {
                            KillBrowserProcesses("firefox");
                        }

                        driver = InitFirefoxDriver(driverDirectory, options);
                    }
                    break;

                default:
                    throw new NotImplementedException("Browser brand: " + browserBrand.ToString());
            }

            driver.Url = startPageUrl.ToString();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(implicitWait);

            return new Context(driver, null, null);
        }

        static ChromeDriver InitChromeDriver(string driverDirectory, DriverOptions options)
        {

            options ??= new ChromeOptions
            {
                PageLoadStrategy = PageLoadStrategy.Normal,
                AcceptInsecureCertificates = true
            };
            ((ChromeOptions)options).AddArgument("--disable-search-engine-choice-screen");
            var driver = driverDirectory != null ? new ChromeDriver(driverDirectory, (ChromeOptions)options) : new ChromeDriver((ChromeOptions)options);
            driver.Manage().Cookies.DeleteAllCookies();
            return driver;
        }

        static EdgeDriver InitEdgeDriver(string driverDirectory, DriverOptions options)
        {
            options ??= new EdgeOptions
            {
                PageLoadStrategy = PageLoadStrategy.Normal,
                UnhandledPromptBehavior = UnhandledPromptBehavior.Accept

            };
            var driver = driverDirectory != null ? new EdgeDriver(driverDirectory, (EdgeOptions)options) : new EdgeDriver((EdgeOptions)options);
            driver.Manage().Cookies.DeleteAllCookies();
            return driver;
        }

        static FirefoxDriver InitFirefoxDriver(string driverDirectory, DriverOptions options)
        {
            options ??= new FirefoxOptions
            {
                PageLoadStrategy = PageLoadStrategy.Normal,
                AcceptInsecureCertificates = true
            };
            var driver = driverDirectory != null ? new FirefoxDriver(driverDirectory, (FirefoxOptions)options) : new FirefoxDriver((FirefoxOptions)options);
            driver.Manage().Cookies.DeleteAllCookies();
            return driver;
        }

        static void KillBrowserProcesses(params string[] processNames)
        {
            foreach (string pname in processNames)
            {
                foreach (Process process in Process.GetProcessesByName(pname))
                {
                    try
                    {
                        process.Kill();
                    }
                    catch (Exception x)
                    {
                        Console.WriteLine(x.ToString());
                    }
                }
            }
        }
    }
}
