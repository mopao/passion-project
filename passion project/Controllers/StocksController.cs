using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Web.Script.Serialization;
using passion_project.Models;

namespace passion_project.Controllers
{
    public class StocksController : Controller
    {
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly HttpClient client;

        //constructor of the controller
        static StocksController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            client = new HttpClient(handler);
            //server address
            client.BaseAddress = new Uri("https://localhost:44328/api/");
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));           

        }
        // GET: Stocks/List
        public ActionResult List()
        {
            // fetch the list of stocks in the system  and send it to the list view
            string url = "StocksData/GetStocks";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<Stock> listStocks = response.Content.ReadAsAsync<IEnumerable<Stock>>().Result;
                return View(listStocks);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Stocks/Details/5
        public ActionResult Details(int id)
        {
            // fetch the stock with the corresponding id and send it to the details view
            string url = "StocksData/GetStock/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                Stock selectedStock = response.Content.ReadAsAsync<Stock>().Result;
               
                // get the item whose the stock is for
                url = "ItemsData/GetItem/" + selectedStock.itemId;
                response = client.GetAsync(url).Result;
                selectedStock.item = response.Content.ReadAsAsync<Item>().Result;
                
                return View(selectedStock);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Stocks/Create/5
        public ActionResult Create(int itemId)
        {
            // fetch the item with the corresponding itemId and send it to the create view
            string url = "ItemsData/GetItem/" + itemId;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                Item selectedItem = response.Content.ReadAsAsync<Item>().Result;
                Stock stock = new Stock();
                stock.itemId = itemId;
                stock.item = selectedItem;
                
                return View(stock);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Stocks/Create
        [HttpPost]
        public ActionResult Create(Stock newStock)
        {
            //insert the new stock
            string url = "StocksData/AddStock";
            HttpContent content = new StringContent(jss.Serialize(newStock));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            //if the insertion was a success it displays the new brand details by redirecting to the details page
            if (response.IsSuccessStatusCode)
            {
                int itemId = response.Content.ReadAsAsync<int>().Result;
                return RedirectToAction("Details","Items",new { id = itemId });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Stocks/Edit/5
        public ActionResult Edit(int id)
        {
            // fetch the stock with corresponding id and send it to the editing view
            string url = "StocksData/GetStock/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                Stock selectedStock = response.Content.ReadAsAsync<Stock>().Result;

                // get the item whose the stock is for
                url = "ItemsData/GetItem/" + selectedStock.itemId;
                response = client.GetAsync(url).Result;
                selectedStock.item = response.Content.ReadAsAsync<Item>().Result;
                return View(selectedStock);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Stocks/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Stock updatedStock)
        {
            //insert the updated stock
            string url = "StocksData/UpdateStock/" + id;
            HttpContent content = new StringContent(jss.Serialize(updatedStock));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            //if the insertion was a success it displays the new brand details by redirecting to the details page
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Stocks/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            // fetch the stock with the corresponding id and send it to the deletion view
            string url = "StocksData/GetStock/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                Stock selectedStock = response.Content.ReadAsAsync<Stock>().Result;
                return View(selectedStock);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Stocks/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, Stock stock)
        {
            //delete the brand 
            string url = "StocksData/DeleteStock/" + id;
            HttpContent content = new StringContent("");//post body is empty
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            // if the deletion was a succes diplay the current brands list
            if (response.IsSuccessStatusCode)
            {

                return RedirectToAction("Details","Items", new { id = stock.itemId});
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
        public ActionResult Error()
        {
            return View();
        }
    }
}
