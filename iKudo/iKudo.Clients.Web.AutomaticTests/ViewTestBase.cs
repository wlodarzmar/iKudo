﻿using Coypu;
using Coypu.Drivers.Selenium;
using NUnit.Framework;
using System;
using System.Configuration;

namespace iKudo.Clients.Web.AutomaticTests
{
    public abstract class TestBase
    {
        public TestBase()
        {
            string appHost = ConfigurationManager.AppSettings["host"].ToString();
            string port = ConfigurationManager.AppSettings["port"].ToString();

            Root = $"{appHost}:{port}";
        }

        public string Root { get; private set; }
    }

    public abstract class ViewTestBase : TestBase
    {
        public BrowserSession Browser { get; private set; }

        [SetUp]
        public virtual void SetUp()
        {
            Browser = GetBrowser();
            Browser.MaximiseWindow();
        }

        private BrowserSession GetBrowser()
        {
            var sessionConfiguration = new SessionConfiguration
            {
                AppHost = ConfigurationManager.AppSettings["host"].ToString(),
                Port = int.Parse(ConfigurationManager.AppSettings["port"].ToString()),
                WaitBeforeClick = new TimeSpan(0, 0, 0, 0, 500),
                ConsiderInvisibleElements = true,
                Match = Match.Single,
            };
            sessionConfiguration.Driver = typeof(SeleniumWebDriver);

            bool isPhantomJS = ConfigurationManager.AppSettings["browser"].ToString() == "phantom";
            if (isPhantomJS)
            {
                sessionConfiguration.Browser = Coypu.Drivers.Browser.PhantomJS;
            }
            else
            {
                sessionConfiguration.Browser = Coypu.Drivers.Browser.Chrome;
            }

            return new BrowserSession(sessionConfiguration);
        }

        [TearDown]
        public virtual void TearDown()
        {
            Browser.Dispose();
        }
    }
}
