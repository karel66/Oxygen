/*
Oxygen Flow library
*/

using System;
using System.Diagnostics;
using System.Threading;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;

namespace Oxygen
{
    public delegate Flow.Context FlowStep(Flow.Context context);

    public partial class Flow
    {
        public static Context CreateContext(Browser browser, System.Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentException(nameof(uri));
            }

            RemoteWebDriver driver = null;

            try
            {
                switch (browser)
                {
                    case Browser.Chrome:
                        {
                            KillBrowserProcesses("chrome");

                            var options = new ChromeOptions
                            {
                                PageLoadStrategy = PageLoadStrategy.Normal,
                                AcceptInsecureCertificates = true
                            };
                            driver = new ChromeDriver(options);
                        }
                        break;

                    case Browser.Edge:
                        {
                            KillBrowserProcesses("MicrosoftEdge", "MicrosoftWebDriver");

                            var options = new EdgeOptions
                            {
                                PageLoadStrategy = PageLoadStrategy.Normal
                            };
                            driver = new EdgeDriver(options);
                        }
                        break;


                    case Browser.FireFox:
                        {
                            //KillBrowserProcesses("firefox");

                            var options = new FirefoxOptions
                            {
                                PageLoadStrategy = PageLoadStrategy.Normal,
                                AcceptInsecureCertificates = true,
                                UseLegacyImplementation = false
                            };
                            driver = new FirefoxDriver(options);
                        }
                        break;


                    case Browser.IE:
                        {
                            KillBrowserProcesses("iexplore");

                            var options = new InternetExplorerOptions
                            {
                                PageLoadStrategy = PageLoadStrategy.Normal,
                                EnableNativeEvents = true,
                                UnhandledPromptBehavior = UnhandledPromptBehavior.Accept,
                                EnablePersistentHover = true,
                                IgnoreZoomLevel = false,
                                IntroduceInstabilityByIgnoringProtectedModeSettings = true,
                                RequireWindowFocus = false,
                            };
                            driver = new InternetExplorerDriver(options);
                        }
                        break;
                }

                driver.Url = uri.ToString();
                return Context.FromDriver(driver);
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
        /// <summary>
        /// Retries the predicate up to 10 times until true
        /// </summary>
        public static string TryAndWait10Times(Func<bool> predicate)
        {
            // Thread.Sleep(100);

            int delay = 0;

            string result = string.Empty;

            for (int i = 1; i < 11; i++)
            {
                try
                {
                    if (predicate())
                    {
                        return string.Empty;
                    }

                    O($"RETRY [{i}]");
                }
                catch (Exception x)
                {
                    O($"RETRY [{i}]: {x.Message} ");
                    result = x.Message;
                }

                delay += 200;
                Thread.Sleep(delay);
            }

            //SaveScreenshot("TryAndWait10Times");

            return result;
        }
    }
}
