using System;
using Coypu;
using System.Drawing;
using NUnit.Framework;
using Coypu.Drivers.Selenium;
using System.Configuration;
using System.IO;

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

        public BrowserSession Browser { get; private set; }

        public string Root { get; private set; }

        [SetUp]
        public void SetUp()
        {
            Browser = GetBrowser();
            Browser.Driver.ResizeTo(new Size(1920, 1280), Browser);
            //string script = File.ReadAllText("loadJqueryScript.js");
            //Browser.ExecuteScript(script);            
            //Browser.LoadJquery();
        }
        
        private BrowserSession GetBrowser()
        {
            var sessionConfiguration = new SessionConfiguration
            {
                AppHost = ConfigurationManager.AppSettings["host"].ToString(),
                Port = int.Parse(ConfigurationManager.AppSettings["port"].ToString()),
                WaitBeforeClick = new TimeSpan(0, 0, 0, 0, 500)
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
