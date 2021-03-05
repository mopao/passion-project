using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using passion_project.Models;

namespace passion_project.Controllers
{
    public class ItemsDataController : ApiController
    {
        private PassionDataContext db = new PassionDataContext();

        /// <summary>
        /// gets all the items in the system
        /// </summary>
        /// <returns> list of items in the database</returns>
        /// <example>GET: api/ItemsData/GetItems</example>

        [ResponseType(typeof(IEnumerable<Item>))]
        public IHttpActionResult GetItems()
        {
            return Ok(db.items.ToList());
        }
        /// <summary>
        /// Gets a list or items in the database alongside a status code (200 OK). Skips the first {startindex} records and takes {nberPerPage} records.
        /// </summary>     
        /// <param name="startIndex">The number of records to skip through</param>
        /// <param name="nberPerPage">The number of records for each page</param
        /// <returns>A list of items</returns>
        /// <example>
        /// GET: api/ItemsData/GetItemsPage/10/5
        /// Retrieves the first 5 items after skipping 20 (third page)
        /// </example>
        [ResponseType(typeof(IEnumerable<Item>))]
        [Route("api/ItemsData/GetItemsPage/{startIndex}/{nberPerPage}")]
        public IHttpActionResult GetItemsPage(int startIndex, int nberPerPage)
        {
            return Ok(db.items.OrderBy(i => i.itemId).Skip(startIndex).Take(nberPerPage).ToList());
        }

        /// <summary>
        /// gets all items associated with a gender in the system.
        /// </summary>
        /// <param name="gender">an item gender</param>
        /// <returns>list of items associated with the gender</returns>
        /// <example>GET: api/ItemsData/GetItemsByGender/1</example>
        [ResponseType(typeof(IEnumerable<Item>))]
        [Route("api/ItemsData/GetItemsByGender/{gender}")]
        public IHttpActionResult GetItemsByGender(int gender)
        {
            return Ok(db.items.Where(s => s.gender == (Gender)gender).ToList());


        }
        /// <summary>
        /// Gets a list of items in the database associated with a gender alongside a status code (200 OK). Skips the first {startindex} records and takes {nberPerPage} records.
        /// </summary>
        /// <param name="gender">an item gender</param>
        /// <param name="startIndex">The number of records to skip through</param>
        /// <param name="nberPerPage">The number of records for each page</param
        /// <returns>list of items associated with the gender</returns>
        /// <example>
        /// Gender: 0=Men and 1= Women
        /// GET: api/ItemsData/GetItemsByGenderPage/0/10/5
        /// Retrieves the first 5 men items after skipping 20 (third page)
        /// </example>
        [ResponseType(typeof(IEnumerable<Item>))]
        [Route("api/ItemsData/GetItemsByGenderPage/{gender}/{startIndex}/{nberPerPage}")]
        public IHttpActionResult GetItemsByGenderPage(int gender, int startIndex, int nberPerPage)
        {
            return Ok(db.items.Where(s => s.gender == (Gender)gender).OrderBy(s => s.itemId).Skip(startIndex).Take(nberPerPage).ToList());


        }

        /// <summary>
        /// gets all items associated with a brand in the system.
        /// </summary>
        /// <param name="brandId">a brand id</param>
        /// <returns>list of items associated with the brand</returns>
        /// <example>GET: api/ItemsData/GetItemsByBrand/1</example>
        [ResponseType(typeof(IEnumerable<Item>))]
        [Route("api/ItemsData/GetItemsByBrand/{brandId}")]
        public IHttpActionResult GetItemsByBrand(int brandId)
        {
            return Ok(db.items.Where(s => s.brandId == brandId).ToList());


        }
        /// <summary>
        /// Gets a list of items in the database associated with a brand alongside a status code (200 OK). 
        /// Skips the first {startindex} records and takes {nberPerPage} records.
        /// </summary>
        /// <param name="brandId">a brand id</param>
        /// <param name="startIndex">The number of records to skip through</param>
        /// <param name="nberPerPage">The number of records for each page</param
        /// <returns>list of items associated with the brand</returns>
        /// <example>
        /// Gender: 0=Men and 1= Women
        /// GET: api/ItemsData/GetItemsByBrandPage/0/10/5
        /// Retrieves the first 5 items associated with the brand after skipping 20 (third page)
        /// </example>
        [ResponseType(typeof(IEnumerable<Item>))]
        [Route("api/ItemsData/GetItemsByBrandPage/{brandId}/{startIndex}/{nberPerPage}")]
        public IHttpActionResult GetItemsByBrandPage(int brandId,int startIndex, int nberPerPage)
        {
            return Ok(db.items.Where(s => s.brandId == brandId).OrderBy(s => s.itemId).Skip(startIndex).Take(nberPerPage).ToList());


        }
        /// <summary>
        /// gets all items associated with a brand and a gender
        /// </summary>
        /// <param name="brandId"> a brand id</param>
        /// <param name="gender"> an item gender</param>
        /// <returns>list of items associated with the brand and the gender</returns>
        /// <example>GET: api/ItemsData/GetItemsByBrandAndGender/1/0</example>
        [ResponseType(typeof(IEnumerable<Item>))]
        [Route("api/ItemsData/GetItemsByBrandAndGender/{brandId}/{gender}")]
        public IHttpActionResult GetItemsByBrandAndGender(int brandId,int gender)
        {
            return Ok(db.items.Where(s => s.brandId == brandId && s.gender == (Gender)gender).ToList());

        }
        /// <summary>
        /// Gets a list of items in the database associated with a brand and a gender alongside a status code (200 OK). 
        /// Skips the first {startindex} records and takes {nberPerPage} records. 
        /// </summary>
        /// <param name="brandId"></param>
        /// <param name="gender"></param>
        /// <param name="startIndex"></param>
        /// <param name="nberPerPage"></param>
        /// <returns>list of items associated with the brand and the gender</returns>
        /// <example>
        /// Gender: 0=Men and 1= Women
        /// GET: api/ItemsData/GetItemsByBrandAndGenderPage/0/10/5
        /// Retrieves the first 5 men items associated the brand after skipping 20 (third page)
        /// </example>
        [ResponseType(typeof(IEnumerable<Item>))]
        [Route("api/ItemsData/GetItemsByBrandAndGenderPage/{brandId}/{gender}/{startIndex}/{nberPerPage}")]
        public IHttpActionResult GetItemsByBrandAndGenderPage(int brandId, int gender, int startIndex, int nberPerPage)
        {
            return Ok(db.items.Where(s => s.brandId == brandId && s.gender == (Gender)gender)
                .OrderBy(s => s.itemId).Skip(startIndex).Take(nberPerPage).ToList());

        }

        /// <summary>
        /// get the item's stocks of an item in the database system.
        /// </summary>
        /// <param name="id"> id of an item</param>
        /// <returns>list of stocks of the item with the provided id  </returns>
        /// <example>GET: api/ItemsData/GetItemStocks/1 </example>
        [ResponseType(typeof(IEnumerable<Stock>))]
        
        public IHttpActionResult GetItemStocks(int id)
        {
            return Ok(db.stocks.Where(s => s.itemId == id && s.quantity > 0).OrderBy(s=> s.size).ToList());
        }
        /// <summary>
        /// gets the item with an specified id in the system
        /// </summary>
        /// <param name="id"> an item id</param>
        /// <returns> the item with the id specified</returns>
        /// <example>GET: api/ItemsData/GetItem/5</example>

        [ResponseType(typeof(Item))]
        public IHttpActionResult GetItem(int id)
        {
            Item item = db.items.Where(i => i.itemId == id).FirstOrDefault<Item>();
            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        /// <summary>
        /// update an item in the system
        /// </summary>
        /// <param name="id"> an item id</param>
        /// <param name="item"> the item to update</param>
        /// <returns>status code 200 if successful.</returns>
        /// <example>POST: api/ItemsData/UpdateItem/5</example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateItem(int id, Item item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != item.itemId)
            {
                return BadRequest();
            }

            db.Entry(item).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Receives item image data and uploads it to the webserver 
        /// </summary>
        /// <param name="id">the item id</param>
        /// <returns>status code 200 if successful.</returns>
        /// <example>
        /// curl -F image=@file.jpg "https://localhost:xx/api/ItemsData/updateItemImage/2"
        /// POST: api/ItemsData/UpdateItemImage/3
        /// HEADER: enctype=multipart/form-data
        /// FORM-DATA: image
        /// </example>
       
        [HttpPost]
        public IHttpActionResult UpdateItemImage(int id)
        {
            
            if (Request.Content.IsMimeMultipartContent())
            {
                Debug.WriteLine("Received multipart form data.");

                int numfiles = HttpContext.Current.Request.Files.Count;
                Debug.WriteLine("Files Received: " + numfiles);

                //Check if a file is posted
                if (numfiles == 1 && HttpContext.Current.Request.Files[0] != null)
                {
                    var itemImg = HttpContext.Current.Request.Files[0];
                    //Check if the file is empty
                    if (itemImg.ContentLength > 0)
                    {
                        var imageTypes = new[] { "jpeg", "jpg", "png", "gif"};
                        var extension = Path.GetExtension(itemImg.FileName).Substring(1);
                        //Check the extension of the file
                        if (imageTypes.Contains(extension))
                        {
                            try
                            {
                                //file name is the id of the image
                                string fn = id + "." + extension;

                                //get a direct file path to ~/Content/Items/{id}.{extension}
                                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Images/"), fn);

                                //save the file
                                itemImg.SaveAs(path);

                                //if these are all successful then we can set the item image value
                                //Update the item image in the database
                                Item SelectedItem = db.items.Find(id);
                                SelectedItem.image = fn;
                                db.Entry(SelectedItem).State = EntityState.Modified;

                                db.SaveChanges();

                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Item Image was not saved successfully.");
                                Debug.WriteLine("Exception:" + ex);
                            }
                        }
                    }

                }
            }

            return Ok();
        }

        /// <summary>
        /// add a new item in the system
        /// </summary>
        /// <param name="item"> item to add</param>
        /// <returns> the added item</returns>
        /// <example>POST: api/ItemsData/AddItem</example>
        [ResponseType(typeof(Item))]
        [HttpPost]
        public IHttpActionResult AddItem(Item item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            item.createdDate = DateTime.Now;
            db.items.Add(item);
            db.SaveChanges();

            return Ok(item.itemId);
        }

        /// <summary>
        /// delete an item in the system
        /// </summary>
        /// <param name="id"> an item id</param>
        /// <returns> the deleted item </returns>
        /// <example>DELETE: api/ItemsData/DeleteItem/5</example>
        [ResponseType(typeof(Item))]
        [HttpPost]
        public IHttpActionResult DeleteItem(int id)
        {
            Item item = db.items.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            //also delete image from path
            string path = HttpContext.Current.Server.MapPath("~/Content/Images/" + item.image);
            if (System.IO.File.Exists(path))
            {
                Debug.WriteLine("File exists... preparing to delete!");
                System.IO.File.Delete(path);
            }            
            /*//remove the item's stocks
            ICollection<Stock> stocks = db.stocks.Where(s => s.itemId == item.itemId).ToList<Stock>();
            foreach(Stock stock in stocks)
            {
                db.stocks.Remove(stock);
            }
            */

            db.items.Remove(item);
            db.SaveChanges();

            return Ok(item);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ItemExists(int id)
        {
            return db.items.Count(e => e.itemId == id) > 0;
        }
    }
}