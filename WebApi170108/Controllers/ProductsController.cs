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
using WebApi170108.Models;

namespace WebApi170108.Controllers
{
    [RoutePrefix("products")]
    public class ProductsController : ApiController
    {
        private FabricsEntities db = new FabricsEntities();

        public ProductsController()
        {
            db.Configuration.LazyLoadingEnabled = false; //關閉延遲載入
        }
        // GET: api/Products
        [Route("")]  //指定走屬性路由
        public IQueryable<Product> GetProduct()
        {
            return db.Product;
        }

        // GET: api/Products/5
        
        [Route("{id:int}", Name ="GetProductById")] // GET : /products/1
        [ResponseType(typeof(Product))] //給http helper參考使用 ，產生文件，該方法不確定回傳型別
        public IHttpActionResult GetProduct(int id)
        {
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }



        //[Route("products/search/{name}")] //GET : /prodcuts/search/will
        //[ResponseType(typeof(Product))]
        //public IHttpActionResult GetProduct(string id)
        //{
        //    Product product = db.Product.Find(id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(product);
        //}

        [Route("search/{name}")] //GET : /prodcuts/search/will
        [ResponseType(typeof(Product))]
        [HttpGet] //Assign Get request , but not restful
        public IHttpActionResult SearchProduct(string name)
        {
            Product product = db.Product.FirstOrDefault(p => p.ProductName .Contains(name));
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

               


        // PUT: api/Products/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutProduct(int id, Product product)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.ProductId)
            {
                return BadRequest();
            }

            db.Entry(product).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // Patch: api/Product/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PatchProduct(int id, Product product)
        {

            //if (!ModelState.IsValid)          檢查modelbinding是否有效，因patch不須此檢查
            //{
            //    return BadRequest(ModelState);
            //}

            if (id != product.ProductId)
            {
                return BadRequest();
            }

            var item = db.Product.Find(id);

            if (!String.IsNullOrEmpty(product.ProductName))  //String not nullable檢查方法
            {
                item.ProductName = product.ProductName;
            }
            if (product.Price.HasValue)
            {
                item.Price = product.Price;
            }
            if (product.Active.HasValue)
            {
                item.Active = product.Active;
            }
            if (product.Active.HasValue)
            {
                item.Stock = product.Stock;
            }

            //db.Entry(product).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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


        // POST: api/Products
        [Route("")]
        [ResponseType(typeof(Product))]
        public IHttpActionResult PostProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Product.Add(product);
            db.SaveChanges();

            return CreatedAtRoute("GetProductById", new { id = product.ProductId }, product);
        }

        // DELETE: api/Products/5
        [ResponseType(typeof(Product))]
        public IHttpActionResult DeleteProduct(int id)
        {
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            db.Product.Remove(product);
            db.SaveChanges();

            return Ok(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductExists(int id)
        {
            return db.Product.Count(e => e.ProductId == id) > 0;
        }
    }
}