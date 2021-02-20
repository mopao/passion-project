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
                                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Items/"), fn);

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
            string path = HttpContext.Current.Server.MapPath("~/Content/Items/" + item.image);
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