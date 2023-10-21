using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using notesApp.API.Data;
using notesApp.API.Models.DataTransferObject;
using notesApp.API.Models.DomainModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using notesApp.API.Utils;
using Azure.Core;
using System.Drawing;

namespace notesApp.API.Controllers
{

   

    [Route("api/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = "Bearer")]

    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly NotesDBCon dbContext;
        public NotesController(NotesDBCon dbContext)
        {
            this.dbContext = dbContext;
        }

        public NotesDBCon DBCon { get; }
        public NotesDBCon DBContext { get; }




        ///<summary>
        ///  This endpoint requires authentication.
        /// </summary>
        /// <remarks>
        ///  A POST methode for creating a new note for this user. 
        /// To access this endpoint, include a valid JWT token in the 'Authorization' header as a bearer token.
        /// Example: 'Authorization: Bearer your-token-here'
        /// </remarks>
        /// <response code="200">Returns a Note object
        ///         {
        ///   "id": "241408c7-95a7-4dbd-90e7-ad9b6623b6f1",
        ///"title": "i postman",
        ///    "description": "postman",
        /// "dateCreated": "2023-10-21T17:46:01.0840649+03:00",
        ///"dateDeleted": null,
        ///    "dateUpdated": null,
        /// "userId": "d126868d-a951-4e34-9cbf-a833ca9acf40",
        ///    "isDeleted": false,
        /// "deletedAt": null
        ///}
        /// </response>
        /// 
        /// <response code="400">If there is no input  </response>
        ///   /// <response code="401">If Invalid Token </response>
        /// <response code="400">If there is no input  their is mistake in the body If this user does not have account or the distenation account is your account (you cant transfer to yourself)</response>
        /// <response code="500">If An error occurred it shows the message</response>
        /// <response code="404">If the url is wrong</response>

        [HttpPost]
        public IActionResult AddNote(AddNoteREquest addnoterequest)
        {
         
            try
            {
                if (addnoterequest == null)
                {
                    return BadRequest("Invalid input. Please providea title, description, or both.");
                }

                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                var note = new Note
                {
                    Title = addnoterequest.Title,
                    Description = addnoterequest.Description,
                    DateCreated = DateTime.Now,
                    UserId = User.FindFirstValue(ClaimTypes.Name)
                };

                dbContext.Notes.Add(note);
                dbContext.SaveChanges();

                return Ok(note);
            }
            catch (Exception ex) 
            {
                return StatusCode(500, $"An error occurred: {ex}");
            }
        }





        ///<summary>
        ///  This endpoint requires authentication.
        /// </summary>
        /// <remarks>
        ///  A GET methode that's returns all the notes for this user. 
        /// To access this endpoint, include a valid JWT token in the 'Authorization' header as a bearer token.
        /// Example: 'Authorization: Bearer your-token-here'
        /// </remarks>
        /// <response code="200">Returns a list of Notes object
        ///         {[
        ///   "id": "241408c7-95a7-4dbd-90e7-ad9b6623b6f1",
        ///"title": "i postman",
        ///    "description": "postman",
        /// "dateCreated": "2023-10-21T17:46:01.0840649+03:00",
        ///"dateDeleted": null,
        ///    "dateUpdated": null,
        /// "userId": "d126868d-a951-4e34-9cbf-a833ca9acf40",
        ///    "isDeleted": false,
        /// "deletedAt": null
        ///]}
        /// </response>
        /// 
        ///   /// <response code="401">If Invalid Token </response>
        /// <response code="500">If An error occurred it shows the message</response>
        /// <response code="404">If the url is wrong</response>

        [HttpGet]
        public IActionResult GetAllNotes()
        {

            try
            {


                var claimsIdentity = this.User.Identity as ClaimsIdentity;

                var userId = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
                if (userId == null)
                {
                    return Unauthorized("User not authenticated.");
                }
                var notes = dbContext.Notes.Where(note => note.UserId == userId && !note.IsDeleted).ToList();

           
                return Ok(notes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex}");
            }
        }







        ///<summary>
        ///  This endpoint requires authentication.
        /// </summary>
        /// <remarks>
        ///  A GET methode returns a note for this user by it's id. 
        /// To access this endpoint, include a valid JWT token in the 'Authorization' header as a bearer token.
        /// Example: 'Authorization: Bearer your-token-here'
        /// </remarks>
        /// <response code="200">Returns a Note object
        ///         {
        ///   "id": "241408c7-95a7-4dbd-90e7-ad9b6623b6f1",
        ///"title": "i postman",
        ///    "description": "postman",
        /// "dateCreated": "2023-10-21T17:46:01.0840649+03:00",
        ///"dateDeleted": null,
        ///    "dateUpdated": null,
        /// "userId": "d126868d-a951-4e34-9cbf-a833ca9acf40",
        ///    "isDeleted": false,
        /// "deletedAt": null
        ///}
        /// </response>
        /// 
        /// <response code="400">If there is no input  or another type</response>
        ///   /// <response code="401">If Invalid Token Or the requested note is not for the same user</response>
        /// <response code="500">If An error occurred it shows the message</response>
        /// <response code="404">If the url is wrong Or the requested note is not exist</response>
        [HttpGet]
        [Route("{id:Guid}")]
        public IActionResult GetNoteById(Guid id)
        {


            try
            {

                var userId = User.FindFirst(ClaimTypes.Name)?.Value; // Get the user's ID from claims

                if (userId == null)
                {
                    return Unauthorized("User not authenticated.");
                }

                var note = dbContext.Notes.Find(id);

                if (note == null)
                {
                    return NotFound("Note not found.");
                }

                if (note.UserId != userId || note.IsDeleted)
                {
                    return Unauthorized("You are not authorized to access this note.");
                }
                return Ok(note);
            }
           
                catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex}");
            }
        

        }


        private string GetUserIdFromToken()
        {
            // Access the current user's claims
            var claims = User.Claims;

            // Find the claim with the user's ID
            var userIdClaim = claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name);

            // Check if the user's ID claim exists
            if (userIdClaim != null)
            {
                return userIdClaim.Value;
            }

            // User's ID not found in claims (unauthenticated or token issue)
            return null;
        }





        ///<summary>
        ///  This endpoint requires authentication.
        /// </summary>
        /// <remarks>
        ///  A PUT methode for editing a note by id for this user . 
        /// To access this endpoint, include a valid JWT token in the 'Authorization' header as a bearer token.
        /// Example: 'Authorization: Bearer your-token-here'
        /// </remarks>
        /// <response code="200">Returns a Note object
        ///         {
        ///   "id": "241408c7-95a7-4dbd-90e7-ad9b6623b6f1",
        ///"title": "i postman",
        ///    "description": "postman",
        /// "dateCreated": "2023-10-21T17:46:01.0840649+03:00",
        ///"dateDeleted": null,
        ///    "dateUpdated": null,
        /// "userId": "d126868d-a951-4e34-9cbf-a833ca9acf40",
        ///    "isDeleted": false,
        /// "deletedAt": null
        ///}
        /// </response>
        /// 
        /// <response code="400">If there is no input  </response>
        ///   /// <response code="401">If Invalid Token </response>
        /// <response code="400">If there is no input or their is mistake in the parametar </response>
        /// <response code="500">If An error occurred it shows the message</response>
        /// <response code="404">If the url is wrong</response>
        [HttpPut]
        [Route("{id:Guid}")]
        public IActionResult UpdateNoteById(Guid id, UpdateNoteRequest updateNoteRequest)
        {
            try
            {

                string userId = GetUserIdFromToken();
                if (userId == null) {
                    return Unauthorized();

                }

                var notedomainobj = dbContext.Notes.Find(id);
                if (notedomainobj != null)
                {
                    if (notedomainobj.UserId == userId)
                    {
                        notedomainobj.Title = updateNoteRequest.Title;
                        notedomainobj.Description = updateNoteRequest.Description;
                        dbContext.SaveChanges();

                        return Ok(notedomainobj);
                    }
                    else
                    {
                        return Unauthorized("You are not authorized to update this note.");
                    }
                
                }
                return BadRequest("Note not found check the id.");

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex}");
            }
        }





        ///<summary>
        ///  This endpoint requires authentication.
        /// </summary>
        /// <remarks>
        ///  A DELETE methode for deleting a note by id for this user. 
        /// To access this endpoint, include a valid JWT token in the 'Authorization' header as a bearer token.
        /// Example: 'Authorization: Bearer your-token-here'
        /// </remarks>
        /// <response code="200">Returns ok succsess message
        /// </response>
        /// 
        /// <response code="400">If there is no input  </response>
        ///   /// <response code="401">If Invalid Token </response>
        /// <response code="400">If there is no input or their is mistake in the parametar </response>
        /// <response code="500">If An error occurred it shows the message</response>
        /// <response code="404">If the url is wrong</response>
        [HttpDelete]
        [Route("{id:Guid}")]
        public IActionResult DeleteNoteById(Guid id)
        {
            try
            {

                string userId = GetUserIdFromToken();
                if (userId == null)
                {
                    return Unauthorized();

                }


                var notedomainobj = dbContext.Notes.Find(id);
                if (notedomainobj != null && notedomainobj.UserId == userId)
                {
                    notedomainobj.IsDeleted = true;
                    notedomainobj.DeletedAt = DateTimeOffset.Now;

                    dbContext.Entry(notedomainobj).State = EntityState.Modified;
                    dbContext.SaveChanges();


                    return Ok();


                }
                return BadRequest("Note not found check the id.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex}");
            }
        }

    }
}
