﻿using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace iKudo.Clients.Web.UITests
{
    public abstract class KudoPage : BaseTest, IDisposable
    {
        public KudoPage()
        {
            Driver = GetDriver();
            Header = new KudoPageHeader(Driver);
        }

        public RemoteWebDriver GetDriver()
        {
            var chromeOptions = new ChromeOptions();
            var arguments = KudoConfiguration.WebDriverArguments.Split(',').Where(x => !string.IsNullOrWhiteSpace(x));
            if (arguments.Any())
            {
                chromeOptions.AddArguments(arguments);
            }

            var driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), chromeOptions);
            driver.Manage().Window.Size = new System.Drawing.Size(1920, 1080);

            return driver;
        }

        public RemoteWebDriver Driver { get; private set; }

        public KudoPageHeader Header { get; private set; }

        public void Dispose()
        {
            Driver.Dispose();
        }
    }
}