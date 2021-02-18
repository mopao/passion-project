﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;
using passion_project.Models;

namespace passion_project.Controllers
{
    public class ItemsController : Controller
    {
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly HttpClient client;

        //constructor of the controller
        static ItemsController()
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
        // GET: Items/List
        public ActionResult List()
        {
            // fetch the list of stocks and send it to the list view
            string url = "ItemsData/GetItems";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<Item> listItems = response.Content.ReadAsAsync<IEnumerable<Item>>().Result;
                return View(listItems);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Items/Details/5
        public ActionResult Details(int id)
        {
            // fetch the item with the  corresponding id and send it to the details view
            string url = "ItemsData/GetItem/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                Item selectedItem = response.Content.ReadAsAsync<Item>().Result;

               /* //retrieve the item's brand
                url = "BrandsData/GetBrand/" + selectedItem.brandId;
                response = client.GetAsync(url).Result;
                selectedItem.brand = response.Content.ReadAsAsync<Brand>().Result;

                //retrieve the item's stocks
                url = "ItemsData/GetItemStocks/" + id;
                response = client.GetAsync(url).Result;
                selectedItem.stocks = response.Content.ReadAsAsync<IEnumerable<Stock>>().Result;
               */

                return View(selectedItem);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Items/Create
        public ActionResult Create()
        {
            // fetch the list of brands and send it to the create view
            string url = "BrandsData/GetBrands";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<Brand> brands = response.Content.ReadAsAsync<IEnumerable<Brand>>().Result;
                return View(brands);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Items/Create
        [HttpPost]
        public ActionResult Create(Item newItem)
        {
            //insert the new Item
            string url = "ItemsData/AddItem";
            HttpContent content = new StringContent(jss.Serialize(newItem));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            //if the insertion was a success it displays the new brand details by redirecting to the details page
            if (response.IsSuccessStatusCode)
            {
                int itemId = response.Content.ReadAsAsync<int>().Result;
                return RedirectToAction("Details", new { id = itemId });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Items/Edit/5
        public ActionResult Edit(int id)
        {
            // fetch the item with the corresponding id and send it to the editing view
            string url = "ItemsData/GetItem/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                Item selectedItem = response.Content.ReadAsAsync<Item>().Result;
                return View(selectedItem);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Items/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Item updatedItem)
        {
            //insert the updated stock
            string url = "ItemsData/UpdateItem/" + id;
            HttpContent content = new StringContent(jss.Serialize(updatedItem));
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

        // GET: Items/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            // fetch the item with the corresponding id and send it to the deletion view
            string url = "ItemsData/GetItem/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                Item selectedItem = response.Content.ReadAsAsync<Item>().Result;
                return View(selectedItem);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Items/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, Item item)
        {
            //delete the item
            string url = "ItemsData/DeleteItem/" + id;
            HttpContent content = new StringContent("");//post body is empty
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            // if the deletion was a succes diplay the current brands list
            if (response.IsSuccessStatusCode)
            {

                return RedirectToAction("List");
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
