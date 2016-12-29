using Coypu;
using System;
using System.IO;
using System.Threading;

namespace iKudo.Clients.Web.AutomaticTests
{
    public static class BrowserExtensions
    {
        public static bool IsActiveRequests(this BrowserSession browser)
        {
            bool exist = false;
            int i = 0;
            do
            {
                Thread.Sleep(100);
                object r = browser.ExecuteScript("return $ !== undefined;");
                exist = bool.Parse(r.ToString());
                i++;

                if (i > 100)
                {
                    throw new System.Exception("Za długo");
                }

            } while (!exist);

            object result = browser.ExecuteScript("return $.active;");
            int activeRequests = int.Parse(result.ToString());

            return activeRequests > 0;
        }

        public static void LoadJquery(this BrowserSession browser)
        {
            //string script = File.ReadAllText("loadJqueryScript.js");
            //browser.ExecuteScript(script);
        }

        public static void WaitForRequests(this BrowserSession browser)
        {
            int i = 0;
            while (browser.IsActiveRequests())
            {
                i++;
                Thread.Sleep(100);
            }

            return;
        }

        public static ElementScope WaitForElementById(this BrowserSession browser, string id)
        {
            int i = 0;
            while (!browser.FindId(id).Exists())
            {
                Thread.Sleep(500);
                if (i > 20)
                {
                    throw new Exception($"Nie znaleziono elementu o identyfikatorze: {id}");
                }
                i++;
            }

            return browser.FindId(id);
        }

        public static void WaitForElementByXpath(this BrowserSession browser, string xpath)
        {
            int i = 0;
            while (!browser.FindXPath(xpath).Exists())
            {
                Thread.Sleep(500);
                if (i > 20)
                {
                    throw new Exception($"Nie znaleziono elementu o ścieżce: {xpath}");
                }
                i++;
            }
        }
    }
}
