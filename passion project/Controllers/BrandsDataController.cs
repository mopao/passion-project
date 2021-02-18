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
    public class BrandsDataController : ApiController
    {
        private PassionDataContext db = new PassionDataContext();

        // GET: api/BrandsData/GetBrands
        [ResponseType(typeof(IEnumerable<Brand>))]
        public IHttpActionResult GetBrands()
        {
            return Ok(db.brands.ToList());
        }

        // GET: api/BrandsData/GetBrand/5
        [ResponseType(typeof(Brand))]
        public IHttpActionResult GetBrand(int id)
        {
            Brand brand = db.brands.Find(id);
            if (brand == null)
            {
                return NotFound();
            }

            return Ok(brand);
        }

        /// <summary>
        /// gets a list of items of the brand specify by an id.
        /// </summary>
        /// <param name="id"> id of a brand </param>
        /// <returns> list of items of the brand</returns>
        /// <example> GET: api/BransDataGetItemsBrand/4 </example>

        [ResponseType(typeof(IEnumerable<Item>))]
        public IHttpActionResult GetItemsBrand(int id)
        {
            return Ok(db.items.Where(i => i.brandId == id).ToList());
        }

        // POST: api/BrandsData/UpdateBrand/5
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateBrand(int id, [FromBody]Brand brand)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != brand.brandId)
            {
                return BadRequest();
            }

            db.Entry(brand).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BrandExists(id))
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

        // POST: api/BrandsData/AddBrand
        [ResponseType(typeof(Brand))]
        [HttpPost]
        public IHttpActionResult AddBrand([FromBody]Brand brand)
        {
            brand.createdDate = DateTime.Now;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.brands.Add(brand);
            db.SaveChanges();

            return  Ok(brand.brandId);
        }

        // DELETE: api/BrandsData/DeleteBrand/5
        [ResponseType(typeof(Brand))]
        [HttpPost]
        public IHttpActionResult DeleteBrand(int id)
        {
            Brand brand = db.brands.Find(id);
            if (brand == null)
            {
                return NotFound();
            }

            db.brands.Remove(brand);
            db.SaveChanges();

            return Ok(brand);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BrandExists(int id)
        {
            return db.brands.Count(e => e.brandId == id) > 0;
        }
    }
}