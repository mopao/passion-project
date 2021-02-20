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


        /// <summary>
        /// gets list of brands in the system       
        /// </summary>
        /// <returns>List of items in the database</returns>
        /// <example> GET: api/BrandsData/GetBrands </example>
        [ResponseType(typeof(IEnumerable<Brand>))]
        public IHttpActionResult GetBrands()
        {
            return Ok(db.brands.OrderBy(b => b.name).ToList());
        }
        /// <summary>
        /// gets the brand with a given id in the system
        /// </summary>
        /// <param name="id"> a brand id</param>
        /// <returns> the brand with the specified id</returns>
        /// <example>GET: api/BrandsData/GetBrand/5</example>

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
        /// <summary>
        /// updates the brand with a specified id in the system
        /// </summary>
        /// <param name="id"> a brand id</param>
        /// <param name="brand"> the brand to update</param>
        /// <returns>status code 200 if successful.</returns>
        /// <example>POST: api/BrandsData/UpdateBrand/5</example>

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
        /// <summary>
        /// adds a brand in the system
        /// </summary>
        /// <param name="brand"> the brand to add</param>
        /// <returns> the brand added</returns>
        /// <example>POST: api/BrandsData/AddBrand</example>

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
        /// <summary>
        /// delete a brand in the database
        /// </summary>
        /// <param name="id"> id of the brand to delete</param>
        /// <returns> the deleted brand in the database</returns>
        /// <example>DELETE: api/BrandsData/DeleteBrand/5</example>

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