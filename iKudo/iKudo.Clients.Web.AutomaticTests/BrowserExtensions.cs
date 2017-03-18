using Coypu;
using System;
using System.Threading;

namespace iKudo.Clients.Web.AutomaticTests
{
    public static class BrowserExtensions
    {
        const int MAXATTEMPTS = 50;
        const int WAITBEFORERETURN = 1000;

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

            Thread.Sleep(WAITBEFORERETURN);

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

            Thread.Sleep(WAITBEFORERETURN);

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

            Thread.Sleep(WAITBEFORERETURN);

            return browser.FindLink(link);
        }

        public static BrowserSession WaitForDialog(this BrowserSession browser, string text)
        {
            int i = 0;
            while (!browser.HasDialog(text, new Options { Match = Match.First, TextPrecision = TextPrecision.Substring }))
            {
                Thread.Sleep(500);
                if (i > MAXATTEMPTS)
                {
                    throw new Exception($"Nie znaleziono okna dialogowego: {text}");
                }
                i++;
            }

            Thread.Sleep(WAITBEFORERETURN);

            return browser;
        }

        public static BrowserSession WaitForText(this BrowserSession browser, string text)
        {
            int i = 0;
            while (!browser.HasContent(text))
            {
                Thread.Sleep(500);
                if (i > MAXATTEMPTS)
                {
                    throw new Exception($"Nie znaleziono okna dialogowego: {text}");
                }
                i++;
            }

            Thread.Sleep(WAITBEFORERETURN);

            return browser;
        }
    }
}
