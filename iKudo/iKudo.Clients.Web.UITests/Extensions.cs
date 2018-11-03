using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace iKudo.Clients.Web.UITests
{
    public static class Extensions
    {
        private const int MaxAttempts = 20;

        public static IWebElement WaitForElement(this RemoteWebDriver driver, By by)
        {
            Wait(driver, by, MaxAttempts);

            return driver.FindElement(by);
        }

        public static IWebElement WaitForElement(this RemoteWebDriver driver, By by, int attempts)
        {
            Wait(driver, by, attempts);

            return driver.FindElement(by);
        }

        public static ReadOnlyCollection<IWebElement> WaitForElements(this RemoteWebDriver driver, By by)
        {
            Wait(driver, by, MaxAttempts);

            return driver.FindElements(by);
        }

        public static ReadOnlyCollection<IWebElement> WaitForElements(this IWebElement element, By by)
        {
            int attempt = 0;
            while (element.FindElements(by).Any() == false && attempt < MaxAttempts)
            {
                Thread.Sleep(1000);
                attempt++;
                Debug.WriteLine($"Attempt: {attempt}, {by.ToString()}");
            }

            FailIfNotFound(attempt, by);

            Debug.WriteLine($"Found element: {by.ToString()}, attempts: {attempt}");

            return element.FindElements(by);
        }

        //TODO: przerobić na webdriverwait https://stackoverflow.com/questions/6992993/selenium-c-sharp-webdriver-wait-until-element-is-present/15142611
        private static void Wait(RemoteWebDriver driver, By by, int attempts)
        {
            int attempt = 0;
            while (driver.FindElements(by).Any() == false && attempt < attempts)
            {
                Thread.Sleep(1000);
                attempt++;
                Console.WriteLine($"Attempt: {attempt}, {by.ToString()}");
            }

            FailIfNotFound(attempt, by, attempts);

            Console.WriteLine($"Found element: {by.ToString()}, attempts: {attempt}");
        }

        private static void FailIfNotFound(int attempt, By by, int attempts = MaxAttempts)
        {
            if (attempt >= attempts)
            {
                throw new System.Exception($"Element '{by.ToString()}' not found");
            }
        }

        public static IWebElement FindParentByClassName(this IWebElement element, string className)
        {
            var parent = element.FindElement(By.XPath("parent::*"));

            if (parent.GetAttribute("class").Contains(className))
            {
                return parent;
            }

            return FindParentByClassName(parent, className);
        }
    }
}
