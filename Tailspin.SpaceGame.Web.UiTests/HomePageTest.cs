using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace Tailspin.SpaceGame.Web.UiTests
{
    [TestFixture("Chrome")]
    [TestFixture("Firefox")]
    [TestFixture("Edge")]
    public class HomePageTest
    {
        private string browser;
        private IWebDriver driver;

        public HomePageTest(string browser)
        {
            this.browser = browser;
        }

        [OneTimeSetUp]
        public void Setup()
        {
            SetupWebDriver(this.browser);
            NavigateToWebSite();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            CloseWebDriver();
        }

        private void CloseWebDriver()
        {
            if (driver != null)
            {
                driver.Close();
                driver.Dispose();
            }
        }

        private void SetupWebDriver(string browser)
        {
            var temp = Environment.GetEnvironmentVariable("TEMP");
            var onedrive = Environment.GetEnvironmentVariable("OneDrive");
            var driver1 = Environment.GetEnvironmentVariable("ChromeWebDriver");
            var driver2 = Environment.GetEnvironmentVariable("GeckoWebDriver");
            var driver3 = Environment.GetEnvironmentVariable("EdgeWebDriver");

            switch (browser)
            {
                case "Chrome":
                    driver = new ChromeDriver(
                        Environment.GetEnvironmentVariable("ChromeWebDriver")
                    );
                    break;
                case "Firefox":
                    driver = new FirefoxDriver(
                        Environment.GetEnvironmentVariable("GeckoWebDriver")
                    );
                    break;
                case "Edge":
                    driver = new EdgeDriver(
                        Environment.GetEnvironmentVariable("EdgeWebDriver")
                    );
                    break;
                default:
                    throw new ArgumentException($"'{browser}': Unknown browser");
            }
        }

        private void NavigateToWebSite()
        {
            string url = Environment.GetEnvironmentVariable("SITE_URL");
            driver.Navigate().GoToUrl(url + "/");
        }

        private IWebElement FindElement(
            By locator, 
            IWebElement parent = null, 
            int timeoutSeconds = 10)
        {
            return new WebDriverWait(
                    driver, 
                    TimeSpan.FromSeconds(timeoutSeconds))
                .Until(c => 
                    FindElement(locator, parent));
        }

        private IWebElement FindElement(By locator, IWebElement parent)
        {
            IWebElement element = parent != null ?
                parent.FindElement(locator) :
                driver.FindElement(locator);

            return (IsElementVisible(element)) ?
                element :
                null;
        }

        private bool IsElementVisible(
            IWebElement element)
        => element != null 
        && element.Displayed 
        && element.Enabled;

        private void ClickElement(IWebElement element)
        => (driver as IJavaScriptExecutor)
             .ExecuteScript("arguments[0].click();", element);

        // Download game
        [TestCase("download-btn", "pretend-modal")]
        // Screen image
        [TestCase("screen-01", "screen-modal")]
        // // Top player on the leaderboard
        [TestCase("profile-1", "profile-modal-1")]
        public void GivenLinkIdWhenLinkClickedThenModalShouldBeDisplayed(
            string linkId, 
            string modalId)
        {
            if (driver == null)
            {
                Assert.Ignore();
                return;
            }

            ClickElement(
                FindElement(
                    By.Id(linkId)));

            IWebElement modal = FindElement(By.Id(modalId));
            bool modalWasDisplayed = IsElementVisible(modal); 
            CloseModalIfDisplayed(modal);

            Assert.That(modalWasDisplayed, Is.True);
        }

        private void CloseModalIfDisplayed(
            IWebElement modal)
        {
            if (IsElementVisible(modal))
            {
                ClickElement(
                    FindElement(
                        By.ClassName("close"), 
                        modal));

                FindElement(
                    By.TagName("body"));
            }
        }
    }
}
