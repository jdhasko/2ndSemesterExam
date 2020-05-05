﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebAPI;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class AuthController : ApiController
    {
        private ParknGardenData db = new ParknGardenData();


        [ResponseType(typeof(string))]
        [Route("api/Auth/GetSalt/{username}")]
        public IHttpActionResult GetSalt(string username)
        {
            string salt = AuthHandler.GetSalt(username, db);
            if (salt == null)
                return NotFound();
            return Ok(salt);
        }

        // GET: api/Auth/Login/username/passwordhash
        [ResponseType(typeof((int UserID, string SessionKey)))]
        [Route("api/Auth/Login/{username}/{password}")]
        public IHttpActionResult GetAuth(string username, string password)
        {
            (int UserID, string SessionKey) sessionInformation = AuthHandler.Login(username, password, db);
            if (sessionInformation.SessionKey == null)
                return NotFound();
            return Ok(sessionInformation);
        }

        // GET: api/Auth/GetUserlevel/sessionkey
        [ResponseType(typeof(int))]
        [Route("api/Auth/GetUserLevel/{sessionKey}")]
        public IHttpActionResult GetUserLevel(string sessionKey)
        {
            int userLevel = AuthHandler.GetUserLevel(sessionKey, db);
            if (userLevel == -1)
                return NotFound();
            return Ok(userLevel);
        }

        // PUT: api/Auths/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAuth(int id, Auth auth)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != auth.UserID)
            {
                return BadRequest();
            }

            db.Entry(auth).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthExists(id))
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

        // POST: api/Auths
        [ResponseType(typeof(Auth))]
        public IHttpActionResult PostAuth(Auth auth)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Auths.Add(auth);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (AuthExists(auth.UserID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = auth.UserID }, auth);
        }

        // DELETE: api/Auths/5
        [ResponseType(typeof(Auth))]
        public IHttpActionResult DeleteAuth(int id)
        {
            Auth auth = db.Auths.Find(id);
            if (auth == null)
            {
                return NotFound();
            }

            db.Auths.Remove(auth);
            db.SaveChanges();

            return Ok(auth);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AuthExists(int id)
        {
            return db.Auths.Count(e => e.UserID == id) > 0;
        }
    }
}