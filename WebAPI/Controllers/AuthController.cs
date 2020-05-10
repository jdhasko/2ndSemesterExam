﻿using System;
using System.Collections.Generic;
using System.Configuration;
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

        // GET: api/Auth/GetUserName
        [ResponseType(typeof(string))]
        [Route("api/Auth/GetUserName/{userID}")]
        public IHttpActionResult GetUserName(int userID)
        {
            string userName = AuthHandler.GetUserName(userID, db);
            if (userName == null)
            {
                return NotFound();
            }

            return Ok(userName);
        }

        // GET: api/Auth/CheckUserName
        [ResponseType(typeof(bool))]
        [Route("api/Auth/CheckUserName/{userName}")]
        public IHttpActionResult GetUserNameExists(string userName)
        {
            if (AuthHandler.CheckUserName(userName, db))
                return Ok(true);
            return NotFound();
        }

        // DELETE: api/Auth/DeleteSession/sessionkey
        [ResponseType(typeof(Session))]
        [Route("api/Auth/DeleteSession/{sessionKey}")]
        public IHttpActionResult DeleteSession(string sessionKey)
        {
            Session session = AuthHandler.DeleteSession(sessionKey, db);
            if (session == null)
                return NotFound();
            return Ok(session);
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

        // POST: api/Auths/PostAuth
        [Route("api/Auth/PostAuth")]
        [ResponseType(typeof(Auth))]
        public async Task<IHttpActionResult> PostAuth(Auth auth)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            AuthHandler.PostNewAuth(auth, db);
            try
            {
                await db.SaveChangesAsync();
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