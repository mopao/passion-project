using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using passion_project.Models;

namespace passion_project.Controllers
{
    public class ItemsDataController : ApiController
    {
        private PassionDataContext db = new PassionDataContext();

        // GET: api/ItemsData/GetItems
        [ResponseType(typeof(IEnumerable<Item>))]
        public IHttpActionResult GetItems()
        {
            return Ok(db.items.Include("brand").ToList());
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
            return Ok(db.stocks.Where(s => s.itemId == id).ToList());
        }

        // GET: api/ItemsData/GetItem/5
        [ResponseType(typeof(Item))]
        public IHttpActionResult GetItem(int id)
        {
            Item item = db.items.Include("brand").Include("stocks").Where(i => i.itemId == id).FirstOrDefault<Item>();
            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        // POST: api/ItemsData/UpdateItem/5
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

        // POST: api/ItemsData/AddItem
        [ResponseType(typeof(Item))]
        [HttpPost]
        public IHttpActionResult AddItem(Item item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.items.Add(item);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = item.itemId }, item);
        }

        // DELETE: api/ItemsData/DeleteItem/5
        [ResponseType(typeof(Item))]
        [HttpPost]
        public IHttpActionResult DeleteItem(int id)
        {
            Item item = db.items.Find(id);
            if (item == null)
            {
                return NotFound();
            }

            db.items.Remove(item);
            //remove the item's stocks
            ICollection<Stock> stocks = db.stocks.Where(s => s.itemId == item.itemId).ToList<Stock>();
            foreach(Stock stock in stocks)
            {
                db.stocks.Remove(stock);
            }

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