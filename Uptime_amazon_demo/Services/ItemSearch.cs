using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;
using Uptime_demo.Models;

namespace Uptime_demo.Services
{
    public class ItemSearch
    {
        private static string MY_AWS_ACCESS_KEY_ID;
        private static string MY_AWS_SECRET_KEY;
        private const string DESTINATION = "ecs.amazonaws.co.uk";



        static ItemSearch()
        {
            var x = new XmlDocument();
            x.Load(System.Web.Hosting.HostingEnvironment.MapPath("~") + "Keys.xml");
            MY_AWS_ACCESS_KEY_ID = x.SelectSingleNode("//AmazonUser/AccessKey").InnerText;
            MY_AWS_SECRET_KEY = x.SelectSingleNode("//AmazonUser/SecretKey").InnerText;
        }

        public List<Item> UpdateBySearch(string searchTerm)
        {
            List<Item> items = new List<Item>();
            int pageCounter = 1;
            XmlDocument resultXml = MakeRequest(searchTerm, pageCounter);
            while (resultXml != null)
            {
                items.AddRange(ProcessResponse(resultXml));
                pageCounter++;
                System.Diagnostics.Debug.WriteLine(items.Count);
                resultXml = MakeRequest(searchTerm, pageCounter);
            }
            return items;
        }


        // from https://msdn.microsoft.com/en-us/library/hh534080.aspx
        private XmlDocument MakeRequest(string searchTerm, int itemPage)
        {


            SignedRequestHelper helper = new SignedRequestHelper(MY_AWS_ACCESS_KEY_ID, MY_AWS_SECRET_KEY, DESTINATION);

            IDictionary<string, string> r1 = new Dictionary<string, String>();
            r1["Service"] = "AWSECommerceService";
            r1["Operation"] = "ItemSearch";
            r1["ResponseGroup"] = "ItemAttributes,OfferSummary";
            r1["AssociateTag"] = "test123";
            r1["Keywords"] = (searchTerm != null) ? searchTerm : "nike";
            r1["SearchIndex"] = "All";
            r1["ItemPage"] = itemPage.ToString();

            string requestUrl = helper.Sign(r1);


            try
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(response.GetResponseStream());
                return (xmlDoc);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        private List<Item> ProcessResponse(XmlDocument toProcess)
        {
            //Create namespace manager

            if (toProcess == null)
            {
                return new List<Item>();
            }


            XmlNamespaceManager nsmgr = new XmlNamespaceManager(toProcess.NameTable);
            nsmgr.AddNamespace("item", "http://webservices.amazon.com/AWSECommerceService/2011-08-01");

            XmlNodeList searchedElements = toProcess.SelectNodes("//item:Item", nsmgr);
            List<Item> items = new List<Item>();
            foreach (XmlNode element in searchedElements)
            {
                items.Add(NewItem(element, nsmgr));
            }
            return items;
        }
        private Item NewItem(XmlNode temp, XmlNamespaceManager nsmgr)
        {
            Item i = new Item();
            if (temp.SelectSingleNode(".//item:ItemAttributes/item:Title", nsmgr) != null)
            {
                i.Title = temp.SelectSingleNode(".//item:ItemAttributes/item:Title", nsmgr).InnerText;
            }
            if (temp.SelectSingleNode(".//item:OfferSummary/item:LowestNewPrice/item:CurrencyCode", nsmgr) != null)
            {
                i.Currency = temp.SelectSingleNode(".//item:OfferSummary/item:LowestNewPrice/item:CurrencyCode", nsmgr).InnerText;
            }
            if (temp.SelectSingleNode(".//item:OfferSummary/item:LowestNewPrice/item:Amount", nsmgr) != null)
            {
                i.Price = decimal.Divide(decimal.Parse(temp.SelectSingleNode(".//item:OfferSummary/item:LowestNewPrice/item:Amount", nsmgr).InnerText), (decimal)100.0);
            }
            if (temp.SelectSingleNode(".//item:DetailPageURL", nsmgr) != null)
            {
                i.Url = temp.SelectSingleNode(".//item:DetailPageURL", nsmgr).InnerText;
            }
            return i;
        }

        public void ChangeRates(List<Item> itemList,decimal rate, string currency)
        {
            foreach (var item in itemList)
            {
                item.Currency = currency;
                item.Price = Math.Round(decimal.Multiply(item.Price, rate), 2);
            }
        }
    }
}