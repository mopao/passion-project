using System;
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
    public class BrandsController : Controller
    {
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly HttpClient client;

        //constructor of the controller
        static BrandsController()
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
        // GET: Brands/List
        public ActionResult List()
        {
            // fetch the list of brands with  and send it to the list view
            string url = "BrandsData/GetBrands";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<Brand> listBrands = response.Content.ReadAsAsync<IEnumerable<Brand>>().Result;
                return View(listBrands);
            }
            else
            {
                return RedirectToAction("Error");
            }
           
        }

        // GET: Brands/Details/5
        public ActionResult Details(int id)
        {
            // fetch the brand with corresponding id and send it to the details view
            string url = "BrandsData/GetBrand/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                Brand selectedBrand = response.Content.ReadAsAsync<Brand>().Result;
                return View(selectedBrand);
            }
            else
            {
                return RedirectToAction("Error");
            }

        }

        // GET: Brands/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Brands/Create
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Create(Brand newBrand)
        {
            //insert the new brand
            string url = "BrandsData/AddBrand"; 
            HttpContent content = new StringContent(jss.Serialize(newBrand));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            
            //if the insertion was a success it displays the new brand details by redirecting to the details page
            if (response.IsSuccessStatusCode)
            {
                int brandId = response.Content.ReadAsAsync<int>().Result;
                return RedirectToAction("Details", new { id = brandId });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Brands/Edit/5
        public ActionResult Edit(int id)
        {   
            // fetch the brand with corresponding id and send it to the editing view
            string url = "BrandsData/GetBrand/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                Brand selectedBrand = response.Content.ReadAsAsync<Brand>().Result;
                return View(selectedBrand);
            }
            else
            {
                return RedirectToAction("Error");
            }         
        }

        // POST: Brands/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Edit(int id, Brand updatedBrand)
        {
            //insert the updated brand
            string url = "BrandsData/UpdateBrand/" + id;
            HttpContent content = new StringContent(jss.Serialize(updatedBrand));
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

        // GET: Brands/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            // fetch the brand with corresponding id and send it to the deletion view
            string url = "BrandsData/GetBrand/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                Brand selectedBrand = response.Content.ReadAsAsync<Brand>().Result;
                return View(selectedBrand);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Brands/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Delete(int id, Brand brand)
        {
            //delete the brand 
            string url = "BrandsData/DeleteBrand/" + id;            
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
