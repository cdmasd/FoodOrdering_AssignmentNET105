using Assignment_Server.Interfaces;
using Assignment_Server.Models;
using Assignment_Server.Models.DTO.User;
using Assignment_Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Assignment_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(UserManager<User> um, ITokenService tokenService) : ControllerBase
    {
        private readonly UserManager<User> _userManager = um;
        private readonly ITokenService _tokenService = tokenService;

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO regis)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new User
                    {
                        UserName = regis.UserName,
                        Email = regis.Email,
                        FullName = regis.FullName
                    };

                    var createdUser = await _userManager.CreateAsync(user, regis.Password);

                    if (createdUser.Succeeded)
                    {
                        var roleResult = await _userManager.AddToRoleAsync(user, "customer");
                        if (roleResult.Succeeded)
                        {
                            return Ok(
                                new UserReturn
                                {
                                    UserName = user.UserName,
                                    Email = user.Email,
                                    FullName = user.FullName,
                                    Token = _tokenService.CreateToken(user)
                                }
                            );
                        }
                        return StatusCode(500, roleResult.Errors);
                    }
                    return StatusCode(500, createdUser.Errors);
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
