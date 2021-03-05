using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;
using passion_project.Models;
using System.Diagnostics;
using passion_project.Models.ViewModel;

namespace passion_project.Controllers
{
    public class ItemsController : Controller
    {
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly HttpClient client;
        private int itemPerRow = 2;
        private int itemPerPage = 4;

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
        public ActionResult List(int pageNum = 1, int gender = -1, int brandId=-1)
        {
            string url;
            string paginatedUrl;

            //build the url for the whole result and the paginated one.
            if(brandId < 1 && gender <= Enum.GetValues(typeof(Gender)).Length -1 && gender >= 0)
            {
                url = "ItemsData/GetItemsByGender/"+gender;
                paginatedUrl = "ItemsData/GetItemsByGenderPage/" + gender;
            }
            else if(brandId > 0 && gender < 0)
            {
                url = "ItemsData/GetItemsByBrand/" + brandId;
                paginatedUrl = "ItemsData/GetItemsByBrandPage/" + brandId;
            }
            else if (brandId > 0 && gender <= Enum.GetValues(typeof(Gender)).Length - 1 && gender >= 0)
            {
                url = "ItemsData/GetItemsByBrandAndGender/" + brandId+"/"+gender;
                paginatedUrl = "ItemsData/GetItemsByBrandPage/" + brandId + "/" + gender;
            }
            else
            {
                url = "ItemsData/GetItems";
                paginatedUrl = "ItemsData/GetItemsPage";
            }

            
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                // --begin of pagination---
                IEnumerable<Item> listItems = response.Content.ReadAsAsync<IEnumerable<Item>>().Result;
                //determines the max number of pages
                int nberItems = listItems.Count();
                int maxPageNber = (int)Math.Ceiling((decimal)nberItems / itemPerPage);
                // Lower boundary for Max Page number
                if (maxPageNber< 1) maxPageNber = 1;
                // Lower boundary for Page Number
                if (pageNum < 1) pageNum = 1;
                // Upper Bound for Page Number
                if (pageNum > maxPageNber) pageNum = maxPageNber;
                // The Record Index of our Page Start
                int startIndex = itemPerPage * (pageNum-1);

                //Helps us generate the HTML which shows "Page 1 of ..." on the list view
                ViewData["PageNum"] = pageNum;
                ViewData["MaxPageNum"] = maxPageNber;
                //ViewData["PageSummary"] = " " + pageNum + " of " + maxPageNber + " ";

                // -- End of Pagination Algorithm --

                // get players according to pagination
                paginatedUrl += "/" + startIndex + "/" + itemPerPage;
                response = client.GetAsync(paginatedUrl).Result;
                listItems = response.Content.ReadAsAsync<IEnumerable<Item>>().Result;
                foreach (Item item in listItems)
                {
                    url = "BrandsData/GetBrand/" + item.brandId;
                    response = client.GetAsync(url).Result;
                    item.brand= response.Content.ReadAsAsync<Brand>().Result;                    
                }                
                ViewBag.itemPerRow = itemPerRow;
                ViewBag.brandId = brandId;
                ViewBag.gender = gender;
                //calculate the number of rows needed to display all the items
                ViewBag.nberOfRows = (int)Math.Ceiling(((decimal)listItems.Count())/itemPerRow);             
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

               //retrieve the item's brand
                url = "BrandsData/GetBrand/" + selectedItem.brandId;
                response = client.GetAsync(url).Result;
                selectedItem.brand = response.Content.ReadAsAsync<Brand>().Result;

                //retrieve the item's stocks
                url = "ItemsData/GetItemStocks/" + id;
                response = client.GetAsync(url).Result;
                selectedItem.stocks = response.Content.ReadAsAsync<IEnumerable<Stock>>().Result;
                

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
                ItemView itemView = new ItemView();
                IEnumerable<Brand> brands = response.Content.ReadAsAsync<IEnumerable<Brand>>().Result;
                List<Gender> genders = new List<Gender>();
                genders.Add(Gender.Men);
                genders.Add(Gender.Women);
                itemView.brands = brands;
                itemView.genders = genders;
                return View(itemView);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Items/Create
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Create(Item newItem, HttpPostedFileBase image)
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
                //Send over image data for player
                url = "ItemsData/updateItemImage/" + itemId;
                Debug.WriteLine("Received item picture " + image.FileName);
                MultipartFormDataContent requestcontent = new MultipartFormDataContent();
                HttpContent imagecontent = new StreamContent(image.InputStream);
                requestcontent.Add(imagecontent, "image", image.FileName);
                response = client.PostAsync(url, requestcontent).Result;
                
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

                //retrieve the item's stocks
                url = "ItemsData/GetItemStocks/" + id;
                response = client.GetAsync(url).Result;
                selectedItem.stocks = response.Content.ReadAsAsync<IEnumerable<Stock>>().Result;

                url = "BrandsData/GetBrands";
                response = client.GetAsync(url).Result;
                
                IEnumerable<Brand> brands = response.Content.ReadAsAsync<IEnumerable<Brand>>().Result;
                List<Gender> genders = new List<Gender>();
                genders.Add(Gender.Men);
                genders.Add(Gender.Women);

                ItemView itemView = new ItemView();
                itemView.brands = brands;
                itemView.genders = genders;
                itemView.item = selectedItem;
                return View(itemView);
                
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Items/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Edit(int id, Item updatedItem, HttpPostedFileBase image, string oldImage)
        {
            //insert the updated stock
            string url = "ItemsData/UpdateItem/" + id;
            if(updatedItem.image == null)
            {
                updatedItem.image = oldImage;
            }
            HttpContent content = new StringContent(jss.Serialize(updatedItem));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            //if the insertion was a success it displays the new brand details by redirecting to the details page
            if (response.IsSuccessStatusCode)
            {
                if(image != null)
                {
                    //Send over image data for player
                    url = "ItemsData/updateItemImage/" + id;
                    Debug.WriteLine("Received item image " + image.FileName);

                    MultipartFormDataContent requestcontent = new MultipartFormDataContent();
                    HttpContent imagecontent = new StreamContent(image.InputStream);
                    requestcontent.Add(imagecontent, "image", image.FileName);
                    response = client.PostAsync(url, requestcontent).Result;
                }
                            
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
        [ValidateAntiForgeryToken()]
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
