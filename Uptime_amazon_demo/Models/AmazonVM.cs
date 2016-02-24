using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Uptime_demo.Models
{
    public class AmazonVM
    {
        public IPagedList<Item> PagedList { get; set; }
        public List<Item> ItemList { get; set; }
        public static Dictionary<string, string> Currencies { get; set; }
        public string SelectedCurrency { get; set; }

        public AmazonVM()
        {
            SelectedCurrency = "GBP";
        }

        public void UpdatePagedList(int page)
        {
            PagedList = ItemList.ToPagedList(page, 13);
        }

        public override string ToString()
        {
            return "Number of Items " + PagedList.Count + ", Selected currency: " + SelectedCurrency;
        }
    }
}