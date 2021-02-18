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
    public class StocksDataController : ApiController
    {
        private PassionDataContext db = new PassionDataContext();

        /// <summary>
        /// gets the list of all the stocks  in the database system.
        /// </summary>
        /// <returns>  the list of all the stocks in the database.</returns>
        /// <example>GET: api/StocksData/GetStocks</example>

        [ResponseType(typeof(IEnumerable<Stock>))]
        public IHttpActionResult GetStocks()
        {
            return Ok(db.stocks.Include("item").ToList());
        }

        // GET: api/StocksData/GetStock/5
        [ResponseType(typeof(Stock))]
        public IHttpActionResult GetStock(int id)
        {
            Stock stock = db.stocks.Include("item").Where(s => s.stockId == id).FirstOrDefault();
            if (stock == null)
            {
                return NotFound();
            }

            return Ok(stock);
        }

        // POST: api/StocksData/UpdateStock/5  
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateStock(int id, Stock stock)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != stock.stockId)
            {
                return BadRequest();
            }

            db.Entry(stock).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StockExists(id))
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

        // POST: api/StocksData/AddStock
        [ResponseType(typeof(Stock))]
        [HttpPost]
        public IHttpActionResult AddStock(Stock stock)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.stocks.Add(stock);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = stock.stockId }, stock);
        }

        // DELETE: api/StocksData/DeleteStock/5
        [ResponseType(typeof(Stock))]
        [HttpPost]
        public IHttpActionResult DeleteStock(int id)
        {
            Stock stock = db.stocks.Find(id);
            if (stock == null)
            {
                return NotFound();
            }

            db.stocks.Remove(stock);
            db.SaveChanges();

            return Ok(stock);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool StockExists(int id)
        {
            return db.stocks.Count(e => e.stockId == id) > 0;
        }
    }
}