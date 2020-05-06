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
        public IQueryable<StockHasItem> GetStockHasItems()
        {
            return db.StockHasItems;
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
        [Route("api/ItemsInStock/{itemID}")]
        public async Task<IHttpActionResult> GetItemInStock(int itemID)
        {

            Dictionary<int, int> items = StockHasItemsHandler.StockHasItems(itemID, db);

            if (items.Count == 0)
                return NotFound();
            return Ok(items);
        }

        // PUT: api/StockHasItems/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutStockHasItem(int id, StockHasItem stockHasItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != stockHasItem.StockID)
            {
                return BadRequest();
            }

            db.Entry(stockHasItem).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StockHasItemExists(id))
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

        // POST: api/StockHasItems
        [ResponseType(typeof(StockHasItem))]
        public async Task<IHttpActionResult> PostStockHasItem(StockHasItem stockHasItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.StockHasItems.Add(stockHasItem);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (StockHasItemExists(stockHasItem.StockID))
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

        // DELETE: api/StockHasItems/5
        [ResponseType(typeof(StockHasItem))]
        public async Task<IHttpActionResult> DeleteStockHasItem(int id)
        {
            StockHasItem stockHasItem = await db.StockHasItems.FindAsync(id);
            if (stockHasItem == null)
            {
                return NotFound();
            }

            db.StockHasItems.Remove(stockHasItem);
            await db.SaveChangesAsync();

            return Ok(stockHasItem);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool StockHasItemExists(int id)
        {
            return db.StockHasItems.Count(e => e.StockID == id) > 0;
        }
    }
}