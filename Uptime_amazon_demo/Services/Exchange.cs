using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using Uptime_demo.Models;

namespace Uptime_demo.Services
{
    public class Exchange
    {

        private static string MY_EXCHANGE_KEY;

        static Exchange()
        {
            var x = new XmlDocument();
            x.Load(System.Web.Hosting.HostingEnvironment.MapPath("~") + "Keys.xml");
            MY_EXCHANGE_KEY = x.SelectSingleNode("//ExchangeUser/AccessKey").InnerText;
        }

        public string MakeJsonRequest(string requestUrl)
        {
            string responseString = null;
            try
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));

                    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        responseString = sr.ReadToEnd();
                    }
                    return responseString;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        public ExchangeJson GetExchange()
        {
            string requestUrl = "https://openexchangerates.org/api/latest.json?app_id=" + MY_EXCHANGE_KEY;
            System.Diagnostics.Debug.WriteLine(requestUrl);
            return JsonConvert.DeserializeObject<ExchangeJson>(MakeJsonRequest(requestUrl));
        }
        public Dictionary<string, string> GetCurrencies()
        {
            string requestUrl = "https://openexchangerates.org/api/currencies.json";
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(MakeJsonRequest(requestUrl));
        }

    }
}