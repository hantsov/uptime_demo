using System.Web.Mvc;
using Uptime_demo.Models;
using Uptime_demo.Services;

namespace Uptime_demo.Controllers
{
    public class AmazonController : Controller
    {
     
        private static ItemSearch itemService = new ItemSearch();
        private static Exchange exchangeService = new Exchange();
        private AmazonVM model;

        // GET: Amazon
        public ActionResult Index(string searchTerm = null, int pageNumber = 1, bool newRequest = true, string currency = null)
        {
            if (Session["model"] != null)
            {
                model = (AmazonVM)Session["model"];
            }
            else
                model = new AmazonVM();
            if (newRequest) {
                model.ItemList = itemService.UpdateBySearch(searchTerm);
                if (AmazonVM.Currencies == null)
                    AmazonVM.Currencies = exchangeService.GetCurrencies();
                if (model.SelectedCurrency != null && !model.SelectedCurrency.Equals("GBP"))
                {
                    UpdateCurrencyModel("GBP", model.SelectedCurrency);
                }
            }
            else if(currency != null)
            {
                UpdateCurrencyModel(model.SelectedCurrency, currency);
            }
            model.UpdatePagedList(pageNumber);
            Session["model"] = model;
            if (Request.IsAjaxRequest())
            {
                return PartialView("_Items", model);
            }
            return View(model);
        }

        private void UpdateCurrencyModel(string baseCurrency, string toCurrency)
        {
            var rateData = exchangeService.GetExchange();
            var rateBase = rateData.Rates[baseCurrency];
            var rateTo = rateData.Rates[toCurrency];
            itemService.ChangeRates(model.ItemList, decimal.Divide(rateTo, rateBase), toCurrency);
            model.SelectedCurrency = toCurrency;
        }

    }
}