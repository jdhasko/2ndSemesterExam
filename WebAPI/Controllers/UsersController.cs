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
    public class UsersController : ApiController
    {
        private ParknGardenData db = new ParknGardenData();

        // GET: api/Users
        [ResponseType((typeof(Dictionary<int, User>)))]
        public IHttpActionResult GetUsers()
        {
            Dictionary<int, User> users = UserHandler.GetAllUsers(db);
            if (users.Count == 0)
            {
                return NotFound();
            }

            return Ok(users);
        }

        // GET: api/Users/5
        [ResponseType(typeof(User))]
        public IHttpActionResult GetUser(int id)
        {
            User user = UserHandler.GetOneUser(id, db);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        // GET: api/Users/FilterById/id
        [ResponseType(typeof(Dictionary<int, User>))]
        [Route("api/Users/FilterByUserLevelId/{id}")]
        public IHttpActionResult GetUsersByLevelId(int id)
        {
            Dictionary<int, User> users = UserHandler.GetUsersByUserLevel(db, id);
            if (users.Count == 0)
            {
                return NotFound();
            }

            return Ok(users);
        }

        // PUT: api/Users/UpdateUser/id/loggedId/sessionKey
        [ResponseType(typeof(void))]
        [Route("api/Users/UpdateUser/{id}/{loggedId}/{sessionKey}")]
        public async Task<IHttpActionResult> PutUser(int id, User user, int loggedId, string sessionKey)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.ID)
            {
                return BadRequest();
            }

            Constants.VerifyUserErrors error = AuthHandler.VerifyUserSession(sessionKey, loggedId, db);
            if (error == Constants.VerifyUserErrors.OK)
            {
                db.Entry(user).State = EntityState.Modified;

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(id))
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

            if (error == Constants.VerifyUserErrors.INCORRECT_SESSION_KEY)
                return StatusCode(HttpStatusCode.Unauthorized);
            if (error == Constants.VerifyUserErrors.SESSION_NOT_FOUND)
                return StatusCode(HttpStatusCode.NotFound);
            return StatusCode(HttpStatusCode.InternalServerError);
        }

        // POST: api/Users
        [ResponseType(typeof(User))]
        public IHttpActionResult PostUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User postedUser = UserHandler.PostUser(db, user);

            return CreatedAtRoute("DefaultApi", new { id = postedUser.ID }, postedUser);
        }

        // DELETE: api/Users/DeleteUser/id
        [Route("api/Users/DeleteUser/{id}")]
        [ResponseType(typeof(User))]
        public IHttpActionResult DeleteUser(int id)
        {
            
            User user = db.Users.FirstOrDefault(u => u.ID == id);
            User returnUser = user;
            
            if (user == null)
            {
                return NotFound();
            }

            if (user.ID != 0)
            {
                UserHandler.DeleteOneUser(db, user);
                returnUser.StoreID = 0;
                return Ok(returnUser);
            }
            
            user.ID = -1;
            return Ok(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(int id)
        {
            return db.Users.Count(e => e.ID == id) > 0;
        }
    }
}