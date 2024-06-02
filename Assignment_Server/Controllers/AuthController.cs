using Assignment_Server.Interfaces;
using Assignment_Server.Models;
using Assignment_Server.Models.DTO.User;
using Assignment_Server.Services;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace Assignment_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(UserManager<User> um, ITokenService tokenService, SignInManager<User> sm) : ControllerBase
    {
        private readonly SignInManager<User> _signInManager = sm;
        private readonly UserManager<User> _userManager = um;
        private readonly ITokenService _tokenService = tokenService;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _userManager.FindByNameAsync(login.UserName);

            if(user == null)
            {
                return Unauthorized("Username and password is not correct!");
            }
            var authen = new AuthenticationProperties() { IsPersistent = login.keepLogined };
            // first params : người dùng có tên đăng nhập là login.UserName (object)
            // second params : mật khẩu mà người dùng nhập  (string)
            // third params : ghi nhớ trạng thái đăng nhập (bool)
            // fourth params : mặc định nếu đăng nhập sai 5 lần liên tiếp sẽ bị khoá đăng nhập trong 5 phút
            var result = await _signInManager.PasswordSignInAsync(user, login.Password,authen.IsPersistent, false);

            if(!result.Succeeded)
            {
                return Unauthorized("Username and password is not correct!");
            }
            var roles = await _userManager.GetRolesAsync(user);
            return Ok(
                    new UserReturn
                    {
                        UserName = user.UserName,
                        Email = user.Email,
                        FullName = user.FullName,
                        Token = _tokenService.CreateToken(user,roles)
                    }
                );
        }

        [HttpPost("register")]
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
                            var roles = await _userManager.GetRolesAsync(user);
                            return Ok(
                                new UserReturn
                                {
                                    UserName = user.UserName,
                                    Email = user.Email,
                                    FullName = user.FullName,
                                    Token = _tokenService.CreateToken(user,roles)
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

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Logout successful" });
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePass(ChangePassDTO changepass)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, changepass.OldPassword, changepass.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return BadRequest(ModelState);
            }

            await _signInManager.RefreshSignInAsync(user);

            return Ok("Your password has been changed.");
        }

        [Authorize]
        [HttpGet("get-info")]
        public async Task<IActionResult> GetInfo()
        {
            var user = await _userManager.GetUserAsync(User);
            var userinfo = new UserInfo
            {
                fullName = user.FullName,
                Address = user.Address,
                avatarUrl = user.AvatarUrl,
                PhoneNumber = user.PhoneNumber
            };
            return Ok(userinfo);
        }

        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] UserInfo userInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _userManager.GetUserAsync(User);
            if(user.FullName != userInfo.fullName)
                user.FullName = userInfo.fullName;

            if (user.Address != userInfo.Address)
                user.Address = userInfo.Address;

            if (user.AvatarUrl != userInfo.avatarUrl)
                user.AvatarUrl = userInfo.avatarUrl;

            if (user.PhoneNumber != userInfo.PhoneNumber)
                user.PhoneNumber = userInfo.PhoneNumber;
            var updateuser = await _userManager.UpdateAsync(user);
            if (updateuser.Succeeded)
            {
                return Ok(new { message = "Updated", user });
            }
            return BadRequest("Update fail");
        }








        [HttpGet("login-google")]
        public IActionResult GoogleLogin()
        {
            var authenticationProperties = new AuthenticationProperties { RedirectUri = Url.Action("signin-google") };
            return Challenge(authenticationProperties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("signin-google")]
        public async Task<IActionResult> GoogleResponse()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return BadRequest("External authentication error");

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new User { UserName = email, Email = email };
                await _userManager.CreateAsync(user);
                await _userManager.AddToRoleAsync(user, "customer");
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true); // Bỏ qua two-factor authentication

            if (signInResult.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                return Ok(new UserReturn
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.FullName,
                    Token = _tokenService.CreateToken(user, roles)
                });
            }
            return BadRequest("Failed to sign in.");
        }

    }
}
