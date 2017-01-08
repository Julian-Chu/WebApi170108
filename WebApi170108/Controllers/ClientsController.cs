using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi170108.Models;

namespace WebApi170108.Controllers
{
    [RoutePrefix("clients")]
    public class ClientsController : ApiController
    {
        public ClientsController()
        {
            db.Configuration.LazyLoadingEnabled = false;
        }

        private FabricsEntities db = new FabricsEntities();

        // GET: api/Clients
        [Route("")]
        [ResponseType(typeof(IQueryable<Client>))]  
        public HttpResponseMessage GetClient()   //使用HttpResponseMessage
        {
            if (!Request.IsLocal())
            {
                //return new HttpResponseMessage()
                //{
                //    StatusCode = HttpStatusCode.NotFound
                //};
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("HttpResponseMessage", Encoding.GetEncoding("Big5"))
            };
        }


        //[Route("")]
        //[ResponseType(typeof(IQueryable<Client>))]  //有時不確定回傳type 須加註 供文件參考
        //public IHttpActionResult GetClient()
        //{
        //    if(!Request.IsLocal())
        //    {
        //        return NotFound();  //使用IHttpActionResult 可控制回傳訊息
        //    }
        //    return Ok(db.Client);
        //}

        #region
        //[Route("")]
        //public IQueryable<Client> GetClient()
        //{
        //    return db.Client;
        //}
        #endregion

        // GET: api/Clients/5
        [Route("{id:int}")]
        [ResponseType(typeof(Client))]
        public IHttpActionResult GetClient(int id)
        {
            Client client = db.Client.Find(id);
            if (client == null)
            {
                return NotFound();
            }

            return Ok(client);
        }
        ///clients/1/orders
        [Route("{id:int}/orders")]
        [ResponseType(typeof(Order))]
        public IHttpActionResult GetClientOrders(int id)
        {
            var orders = db.Order.Where(p => p.ClientId == id);
            return Ok(orders.ToList());
        }

        ///clients/1/orders/2017-01-08
        [Route("{id:int}/orders/{*date:datetime}")]
        [ResponseType(typeof(Order))]
        public IHttpActionResult GetClientOrdersByDate(int id, DateTime date)
        {
            var orders = db.Order.Where(p => p.ClientId == id);
            var day_begin = date.Date;
            var day_after = date.AddDays(1).Date;

            return Ok(orders.Where(p => p.OrderDate.Value > day_begin && p.OrderDate.Value < day_after));
        }

        //clients/1/orders/top10
        [Route("{id:int}/orders/top10")]
        [ResponseType(typeof(Order))]
        public IHttpActionResult GetClientOrdersByTop10(int id)
        {
            var orders = db.Order.Where(p => p.ClientId == id).OrderByDescending(p => p.OrderDate).Take(10);

            return Ok(orders);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ClientExists(int id)
        {
            return db.Client.Count(e => e.ClientId == id) > 0;
        }
    }
}