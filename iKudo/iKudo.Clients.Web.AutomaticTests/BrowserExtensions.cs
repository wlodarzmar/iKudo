using Coypu;
using System;
using System.IO;
using System.Threading;

namespace iKudo.Clients.Web.AutomaticTests
{
    public static class BrowserExtensions
    {
        const int MAXATTEMPTS = 50;
        
        public static ElementScope WaitForElementById(this BrowserSession browser, string id)
        {
            int i = 0;
            while (!browser.FindId(id).Exists())
            {
                Thread.Sleep(500);
                if (i > MAXATTEMPTS)
                {
                    throw new Exception($"Nie znaleziono elementu o identyfikatorze: {id}");
                }
                i++;
            }

            return browser.FindId(id);
        }

        public static ElementScope WaitForElementByXpath(this BrowserSession browser, string xpath)
        {
            int i = 0;
            while (!browser.FindXPath(xpath).Exists())
            {
                Thread.Sleep(500);
                if (i > MAXATTEMPTS)
                {
                    throw new Exception($"Nie znaleziono elementu o ścieżce: {xpath}");
                }
                i++;
            }

            return browser.FindXPath(xpath);
        }

        public static ElementScope WaitForLink(this BrowserSession browser, string link)
        {
            int i = 0;
            while (!browser.FindLink(link).Exists())
            {
                Thread.Sleep(500);
                if (i > MAXATTEMPTS)
                {
                    throw new Exception($"Nie znaleziono linku: {link}");
                }
                i++;
            }

            return browser.FindLink(link);
        }
    }
}
