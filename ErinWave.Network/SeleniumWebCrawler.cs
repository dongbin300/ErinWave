//-----------------------------------------------------------------------
//
// MIT License
//
// Copyright (c) 2025 Erin Wave
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//-----------------------------------------------------------------------

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace ErinWave.Network
{
    public class SeleniumWebCrawler
    {
        static IWebDriver driver = default!;
        public static string Source => driver.PageSource;
        public static bool CreateNoWindow = false;

        public static void Open(string url = "", bool createNoWindow = false)
        {
            CreateNoWindow = createNoWindow;
            var driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = CreateNoWindow;
            var options = new ChromeOptions();
            if (CreateNoWindow)
            {
                options.AddArgument("headless");
            }
            driver = new ChromeDriver(driverService, options);

            if (!string.IsNullOrEmpty(url))
            {
                SetUrl(url);
            }
        }

        public static void OpenDeep(string url = "")
        {
			CreateNoWindow = true;
			var driverService = ChromeDriverService.CreateDefaultService();
			driverService.HideCommandPromptWindow = CreateNoWindow;
			var options = new ChromeOptions();
			if (CreateNoWindow)
			{
				options.AddArgument("headless");
			}
			options.AddArgument("--disable-blink-features=AutomationControlled");
			options.AddExcludedArgument("enable-automation");
			options.AddAdditionalOption("useAutomationExtension", false);
			options.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36");
			driver = new ChromeDriver(driverService, options);
			ExecuteScript("Object.defineProperty(navigator, 'webdriver', {get: () => undefined})");

			if (!string.IsNullOrEmpty(url))
			{
				SetUrl(url);
			}
		}

        public static void Close()
        {
            driver.Dispose();
        }

        public static void Refresh()
        {
            driver.Navigate().Refresh();
        }

        public static void GoToUrl(string url)
		{
			driver.Navigate().GoToUrl(url);
		}

		public static void SetUrl(string url)
        {
            driver.Url = url;
        }

        public static ITargetLocator SwitchTo()
        {
            return driver.SwitchTo();
        }

        public static object? ExecuteScript(string script, params object[] args)
        {
            var js = (IJavaScriptExecutor)driver;
            return js.ExecuteScript(script, args);
        }

        public static bool WaitForVisible(string tag, string attribute, string argument, bool isContain = false, int seconds = 10)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(seconds));

            try
            {
                var element = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(ToBy(tag, attribute, argument, isContain)));
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static IWebElement SelectNode(string xpath)
        {
            return driver.FindElement(By.XPath(xpath));
        }

        public static IWebElement SelectNode(IWebElement node, string xpath)
        {
            return node.FindElement(By.XPath(xpath));
        }

        /// <summary>
        /// //tag[@attribute='argument']
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="attribute"></param>
        /// <param name="argument"></param>
        /// <param name="isContain"></param>
        /// <returns></returns>
        public static IWebElement SelectNode(string tag, string attribute, string argument, bool isContain = false)
        {
            return SelectNode(ToXPath(tag, attribute, argument, isContain));
        }

        /// <summary>
        /// //tag[@attribute='argument']
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="attribute"></param>
        /// <param name="argument"></param>
        /// <param name="isContain"></param>
        /// <returns></returns>
        public static IWebElement SelectNode(IWebElement node, string tag, string attribute, string argument, bool isContain = false)
        {
            return SelectNode(node, ToXPath(tag, attribute, argument, isContain));
        }

        public static IEnumerable<IWebElement> SelectNodes(string xpath)
        {
            return driver.FindElements(By.XPath(xpath));
        }

        public static IEnumerable<IWebElement> SelectNodes(IWebElement node, string xpath)
        {
            return node.FindElements(By.XPath(xpath));
        }

        /// <summary>
        /// //tag[@attribute='argument']
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="attribute"></param>
        /// <param name="argument"></param>
        /// <param name="isContain"></param>
        /// <returns></returns>
        public static IEnumerable<IWebElement> SelectNodes(string tag, string attribute, string argument, bool isContain = false)
        {
            return SelectNodes(ToXPath(tag, attribute, argument, isContain));
        }

        /// <summary>
        /// //tag[@attribute='argument']
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="attribute"></param>
        /// <param name="argument"></param>
        /// <param name="isContain"></param>
        /// <returns></returns>
        public static IEnumerable<IWebElement> SelectNodes(IWebElement node, string tag, string attribute, string argument, bool isContain = false)
        {
            return SelectNodes(node, ToXPath(tag, attribute, argument, isContain));
        }

        public static string ToXPath(string tag, string attribute, string argument, bool isContain = false)
        {
            //".//div[@class='cmt_contents']"
            //"//table[contains(@id,'table-dark')]"
            return isContain ?
                $".//{tag}[contains(@{attribute}, '{argument}')]" :
                $".//{tag}[@{attribute}='{argument}']";
        }

        public static By ToBy(string tag, string attribute, string argument, bool isContain = false)
        {
            return By.XPath(ToXPath(tag, attribute, argument, isContain));
        }

        public static void SendKeys(string xpath, string text)
		{
			var element = SelectNode(xpath);
			element.SendKeys(text);
		}

        public static void Click(string xpath)
        {
			var element = SelectNode(xpath);
			element.Click();
		}
	}
}