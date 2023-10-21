using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

using notesApp.API.Data;
using notesApp.API.Models.DataTransferObject;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using notesApp.API.Utils;

namespace notesApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        
        private readonly IConfiguration _configuration;
        private readonly NotesDBCon dbContext;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public UserController(IConfiguration configuration, NotesDBCon dbContext, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _configuration = configuration;
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }


        public NotesDBCon DBCon { get; }
        public NotesDBCon DBContext { get; }




        /// <remarks>
        ///  A POST methode for creating a new user (registreation proccess).
        /// </remarks>
        /// <response code="200">Returns 
        ///        {
        ///          "succeeded": true,
        ///       "errors": []
        ///       }
        /// </response>
        /// <response code="400">If Email or password is missing OR their is mistake in the input </response>
        /// <response code="409">If Email already exist (conflict)</response>
        // POST: api/User/Register
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            try
            {
                // Ensure that the model contains the necessary properties, including 'Password'
                if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                {
                    return BadRequest("Invalid input. Please provide email and password.");
                }

                // Check if the username is already in use
                if (dbContext.Users.Any(u => u.Email == model.Email))
                {
                    return Conflict("Email already in use.");
                }
                var UserName = model.Email;

                if ( model.UserName != null)
                {
                    UserName = model.UserName;

                }
                var user = new User
                {
                    Email = model.Email,
                    UserName = UserName,
                };
          
                var res = await userManager.CreateAsync(user, model.Password);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
             
            }
        }







        /// <remarks>
        ///  A POST methode for logging in and returning a token.
        /// </remarks>


        /// <response code="200">Returns 
        ///  {"token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiZmJjZWRhZDUtMTE5YS00MGY4LThkMGMtNDBiMDVlMzFiODI5IiwibmJmIjoxNjk3ODUxNTc3LCJleHAiOjE2OTc4NTUxNzcsImlhdCI6MTY5Nzg1MTU3NywiaXNzIjoiaXNzdWVyIiwiYXVkIjoiYXVkaWVuY2UifQ.qdK8rLbDdV8TPkQ_mLiI4rH1BiIfTRETSddztaHV7X4"}
        /// </response>
        /// <response code="400"> if not providding both email and password  OR their is mistake in the input </response>
        /// <response code="401"> if Invalid Email or password   OR their is mistake in the input </response>
        /// <response code="500">If An error occurred it shows the message</response>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            try
            {

                if (login == null || string.IsNullOrEmpty(login.Email) || string.IsNullOrEmpty(login.Password))
                {
                    return BadRequest("Invalid input. Pleaseeeee provide both email and password.");
                }


                //var _user = dbContext.Users.FirstOrDefault(x => x.UserName == login.Username && x.Password == login.Password);

                var user = await userManager.FindByEmailAsync(login.Email); // Use userManager

                if (user == null)
                {
                    return Unauthorized("Invalid Email.");
                }

                var result = await userManager.CheckPasswordAsync(user, login.Password); // Use userManager
                //var result = BCrypt.Net.BCrypt.Verify(login.Password, user);

                if (result)
                {
                    var token = GenerateToken(user, EncreptionClass.DecryptContent(_configuration["Jwt:Key"]));
                    return Ok(new { token });
                }
                else
                {
                    return Unauthorized("Invalid Email or password.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex}");
            }
        }










        private string GenerateToken(User user, string secretKey)
        {
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, user.Id)
            }),

                Expires = DateTime.UtcNow.AddHours(1), // Token expiration time
                SigningCredentials = credentials,
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],


            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}