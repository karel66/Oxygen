/*
* Oxygen.Flow library
* by karel66, 2023
*/

using System;
using System.Diagnostics;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;

namespace Oxygen
{
    /// <summary>
    /// Base class for Selenium UI tests.
    /// </summary>
    public partial class Flow
    {
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
            if (startPageUrl == null) throw new ArgumentException($"{nameof(CreateContext)}: NULL argument: {nameof(startPageUrl)}");

            WebDriver driver = null;

            try
            {
                switch (browserBrand)
                {
                    case BrowserBrand.Chrome:
                        {
                            if (killOthers) KillBrowserProcesses("chrome");

                            if (options == null)
                            {
                                options = new ChromeOptions
                                {
                                    PageLoadStrategy = PageLoadStrategy.Normal,
                                    AcceptInsecureCertificates = true
                                };
                            }

                            driver = driverDirectory != null ? new ChromeDriver(driverDirectory, (ChromeOptions)options) : new ChromeDriver((ChromeOptions)options);
                        }
                        break;

                    case BrowserBrand.Edge:
                        {
                            if (killOthers) KillBrowserProcesses("MicrosoftEdge", "MicrosoftWebDriver");

                            if (options == null)
                            {
                                options = new EdgeOptions
                                {
                                    PageLoadStrategy = PageLoadStrategy.Normal,
                                    UnhandledPromptBehavior = UnhandledPromptBehavior.Accept

                                };
                            }
                            driver = driverDirectory != null ? new EdgeDriver(driverDirectory, (EdgeOptions)options) : new EdgeDriver((EdgeOptions)options);
                        }
                        break;


                    case BrowserBrand.FireFox:
                        {
                            if (killOthers) KillBrowserProcesses("firefox");

                            if (options == null)
                            {
                                options = new FirefoxOptions
                                {
                                    PageLoadStrategy = PageLoadStrategy.Normal,
                                    AcceptInsecureCertificates = true
                                };
                            }
                            driver = driverDirectory != null ? new FirefoxDriver(driverDirectory, (FirefoxOptions)options) : new FirefoxDriver((FirefoxOptions)options);
                        }
                        break;


                    case BrowserBrand.IE:
                        {
                            if (killOthers) KillBrowserProcesses("iexplore");

                            if (options == null)
                            {
                                options = new InternetExplorerOptions
                                {
                                    PageLoadStrategy = PageLoadStrategy.Normal,
                                    EnableNativeEvents = true,
                                    UnhandledPromptBehavior = UnhandledPromptBehavior.Accept,
                                    EnablePersistentHover = true,
                                    IgnoreZoomLevel = true,
                                    EnsureCleanSession = false,
                                    IntroduceInstabilityByIgnoringProtectedModeSettings = true,
                                    RequireWindowFocus = false

                                };
                            }
                            driver = driverDirectory != null ? new InternetExplorerDriver(driverDirectory, (InternetExplorerOptions)options) : new InternetExplorerDriver((InternetExplorerOptions)options);
                        }
                        break;
                }

                driver.Url = startPageUrl.ToString();

                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(implicitWait);

                return new Context(driver, null, null);
            }
            catch (Exception x)
            {
                return new Context(null, null, null, x);
            }
        }

        static void KillBrowserProcesses(params string[] processNames)
        {
            foreach (var pname in processNames)
            {
                foreach (var process in Process.GetProcessesByName(pname))
                {
                    try
                    {
                        process.Kill();
                    }
                    catch { }
                }
            }
        }
    }
}
