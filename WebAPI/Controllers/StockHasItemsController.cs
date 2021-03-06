﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebAPI;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class StockHasItemsController : ApiController
    {
        private ParknGardenData db = new ParknGardenData();

        // GET: api/StockHasItems
        [ResponseType(typeof(Dictionary<int, Dictionary<int, int>>))]
        public IHttpActionResult GetStockHasItems()
        {
            Dictionary<int, Dictionary<int, int>> items = StockHasItemsHandler.StockHasItems(db);

            if (items.Count == 0)
                return NotFound();
            return Ok(items);
        }

        // GET: api/StockHasItems/5
        [ResponseType(typeof(Dictionary<int, int>))]
        public async Task<IHttpActionResult> GetStockHasItem(int id)
        {

            Dictionary<int, int> items = StockHasItemsHandler.StockHasItems(id, db);

            if (items.Count == 0)
                return NotFound();
            return Ok(items);
        }

        // GET: api/ItemsInStock/5
        [ResponseType(typeof(Dictionary<int, int>))]
        [Route("api/ItemsInStocks/{itemID}")]
        public async Task<IHttpActionResult> GetItemInStock(int itemID)
        {

            Dictionary<int, int> items = StockHasItemsHandler.ItemsInStock(itemID, db);

            if (items.Count == 0)
                return NotFound();
            return Ok(items);
        }
        // GET: api/ItemsInStock
        [ResponseType(typeof(Dictionary<int, Dictionary<int, int>>))]
        [Route("api/ItemsInStocks")]
        public IHttpActionResult GetItemsInStocks()
        {
            Dictionary<int, Dictionary<int, int>> items = StockHasItemsHandler.ItemsInStock(db);

            if (items.Count == 0)
                return NotFound();
            return Ok(items);
        }
        // PUT: api/StockHasItems/5/5/loggedId/sessionKey
        [ResponseType(typeof(void))]
        [Route("api/StockHasItems/{stockID}/{itemID}/{loggedId}/{sessionKey}")]
        public async Task<IHttpActionResult> PutStockHasItem(int stockID, int itemID, StockHasItem stockHasItem, int loggedId, string sessionKey)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (stockID != stockHasItem.StockID || itemID != stockHasItem.ItemID)
            {
                return BadRequest();
            }

            Constants.VerifyUserErrors error = AuthHandler.VerifyUserSession(sessionKey, loggedId, db);
            if (error == Constants.VerifyUserErrors.OK)
            {
                db.Entry(stockHasItem).State = EntityState.Modified;

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockHasItemExists(stockID, itemID))
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

            return StatusCode(CommonMethods.StatusCodeReturn(error));
        }

        // POST: api/StockHasItems/loggedId/sessionKey
        [Route("api/StockHasItems/{loggedId}/{sessionKey}")]
        [ResponseType(typeof(StockHasItem))]
        public async Task<IHttpActionResult> PostStockHasItem(StockHasItem stockHasItem, int loggedId, string sessionKey)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Constants.VerifyUserErrors error = AuthHandler.VerifyUserSession(sessionKey, loggedId, db);
            if (error == Constants.VerifyUserErrors.OK)
            {
                db.StockHasItems.Add(stockHasItem);

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    if (StockHasItemExists(stockHasItem.StockID, stockHasItem.ItemID))
                    {
                        return Conflict();
                    }
                    else
                    {
                        throw;
                    }
                }

                return CreatedAtRoute("DefaultApi", new { id = stockHasItem.StockID }, stockHasItem);
            }

            return StatusCode(CommonMethods.StatusCodeReturn(error));
        }

        // DELETE: api/StockHasItems/5/loggedId/sessionKey
        [Route("api/StockHasItems/5/{loggedId}/{sessionKey}")]
        [ResponseType(typeof(StockHasItem))]
        public async Task<IHttpActionResult> DeleteStockHasItem(int id, int loggedId, string sessionKey)
        {
            Constants.VerifyUserErrors error = AuthHandler.VerifyUserSession(sessionKey, loggedId, db);
            StockHasItem stockHasItem = await db.StockHasItems.FindAsync(id);
            if (error == Constants.VerifyUserErrors.OK)
            {
                if (stockHasItem == null)
                {
                    return NotFound();
                }

                db.StockHasItems.Remove(stockHasItem);
                await db.SaveChangesAsync();

                return Ok(stockHasItem);
            }

            return StatusCode(CommonMethods.StatusCodeReturn(error));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool StockHasItemExists(int stockID, int itemID)
        {
            return db.StockHasItems.Count(e => e.StockID == stockID && e.ItemID == itemID) > 0;
        }
    }
}