/*
Oxygen Flow library
*/

using System;
using System.Diagnostics;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;

namespace Oxygen
{
    /// <summary>
    /// Base class for tests.
    /// </summary>
    public partial class Flow
    {
        /// <summary>
        /// Instantiates selected browser.
        /// </summary>
        /// <param name="browserBrand">Browser brand to instantiate.</param>
        /// <param name="startPageUrl">Satrt page URL.</param>
        /// <param name="killOthers">Optional. If true then other browser instances are closed.</param>
        /// <param name="driverDirectory">Optional driver directory. If not spcefied then environment PATH is used.</param>
        /// <param name="options">Specific driver options. Must match the browser brand options type.</param>
        /// <returns>Driver in context</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public static Context CreateContext(BrowserBrand browserBrand, Uri startPageUrl, bool killOthers = false, string driverDirectory = null, DriverOptions options = null)
        {
            if (startPageUrl == null) throw new ArgumentException($"{nameof(CreateContext)}: NULL argument: {nameof(startPageUrl)}");

            RemoteWebDriver driver = null;

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
                                    PageLoadStrategy = PageLoadStrategy.Normal
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
                                    AcceptInsecureCertificates = true,
                                    UseLegacyImplementation = false
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
                                    IgnoreZoomLevel = false,
                                    IntroduceInstabilityByIgnoringProtectedModeSettings = true,
                                    RequireWindowFocus = false,
                                };
                            }
                            driver = driverDirectory != null ? new InternetExplorerDriver(driverDirectory, (InternetExplorerOptions)options) : new InternetExplorerDriver((InternetExplorerOptions)options);
                        }
                        break;
                }

                driver.Url = startPageUrl.ToString();

                return new Context(driver, null, null);
            }
            catch (Exception x)
            {
                return new Context(null, null, null, x);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
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
