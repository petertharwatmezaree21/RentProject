using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyProject.Helpers;
using MyProject.Models;

namespace MyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _UserManger;
        private readonly SignInManager<IdentityUser> _SignInManager;
        private readonly AppSettings _appSettings;

        public AccountController(UserManager<IdentityUser> UserManger, SignInManager<IdentityUser> SignInManager,IOptions<AppSettings> appSettings)
        {
            this._SignInManager = SignInManager;
            this._UserManger = UserManger;
            this._appSettings = appSettings.Value;
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel formData)
        {
            List<string> ErrorList = new List<string>();

            var user = new IdentityUser
            {
                Email = formData.Email,
                UserName = formData.UserName,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var request = await _UserManger.CreateAsync(user, formData.Password);

            if(request.Succeeded)
            {
                await _UserManger.AddToRoleAsync(user, "Customer");
                //Here we will store this info in LocalStorage
                return Ok(new { username = user.UserName, email = user.Email,status=1,message="Registration Successfully" });
            }

            else
            {
                foreach (var error in request.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    ErrorList.Add(error.Description);

                }
            }

            return BadRequest(new JsonResult(ErrorList));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login( [FromBody]LoginViewModel formData)
        {
            var user = await _UserManger.FindByNameAsync(formData.UserName);
            var userRole = await _UserManger.GetRolesAsync(user);
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.Secret));
            double tokenExpiryDate = Convert.ToDouble(_appSettings.ExpirenceTime);
            if (user !=null && await _UserManger.CheckPasswordAsync(user, formData.Password))
            {
                // Here we Want to Check ConfirmationEmail

                

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor {
                    Subject= new ClaimsIdentity(new Claim[] {
                        new Claim(JwtRegisteredClaimNames.Sub,formData.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                        new Claim(ClaimTypes.NameIdentifier,user.Id),
                        new Claim(ClaimTypes.Role,userRole.FirstOrDefault()),
                        new Claim("LoggedOn",DateTime.Now.ToString())
                    }),
                    SigningCredentials=new SigningCredentials(key,SecurityAlgorithms.HmacSha256Signature),
                    Issuer=_appSettings.Site,
                    Audience=_appSettings.Audience,
                    Expires=DateTime.UtcNow.AddMinutes(tokenExpiryDate)

                };

                //then Generate Token

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return Ok(new {token=tokenHandler.WriteToken(token),expiration=token.ValidTo,username=user.UserName,userRole=userRole.FirstOrDefault() });

            }
            ModelState.AddModelError("", "UserName/Password are wrong");
            return Unauthorized(new { LoginError="Please Check the Creadential - InValid UserName/Password was Entered" });

        }
            
    }
}