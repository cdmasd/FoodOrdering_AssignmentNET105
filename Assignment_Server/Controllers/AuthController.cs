using Assignment_Server.Interfaces;
using Assignment_Server.Models;
using Assignment_Server.Models.DTO.User;
using Assignment_Server.Services;
using CloudinaryDotNet.Actions;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace Assignment_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(UserManager<User> um, ITokenService tokenService, SignInManager<User> sm, IConfiguration config) : ControllerBase
    {
        private readonly SignInManager<User> _signInManager = sm;
        private readonly UserManager<User> _userManager = um;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IConfiguration _config = config;

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

        [HttpGet("google-login")]
        public IActionResult LoginWithGoogle()
        {
            var domain = _config["Auth0:Domain"];
            var clientId = _config["Auth0:ClientId"];
            var redirectUri = "https://localhost:7294/api/Auth/getcode";

            var loginUrl = $"https://{domain}/authorize?response_type=code&client_id={clientId}&redirect_uri={redirectUri}&scope=openid%20profile%20email";
            return Ok(loginUrl);
        }

        [HttpGet("getcode")]
        public async Task<IActionResult> GetCode(string code)
        {
            return Redirect($"https://localhost:7211/Auth/CallBack?code={code}");
        }

        [HttpGet("callback")]
        public async Task<IActionResult> CallBack(string code)
        {
            var domain = _config["Auth0:Domain"];
            var clientId = _config["Auth0:ClientId"];
            var clientSecret = _config["Auth0:ClientSecret"];
            var redirectUri = "https://localhost:7294/api/Auth/callback";

            var tokenClient = new HttpClient();
            var tokenResponse = await tokenClient.PostAsync($"https://{domain}/oauth/token", new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", redirectUri)
            }));

            var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
            var tokenData = JsonConvert.DeserializeObject<Dictionary<string, object>>(tokenContent);
            if (tokenData != null && tokenData.ContainsKey("id_token"))
            {
                var idToken = tokenData["id_token"];
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(idToken.ToString()) as JwtSecurityToken;
                var email = jsonToken.Claims.First(claim => claim.Type == "email").Value;
                var FullName = jsonToken.Claims.First(claim => claim.Type == "nickname").Value;
                var picture = jsonToken.Claims.First(claim => claim.Type == "picture").Value;

                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new User
                    {
                        UserName = FullName,
                        Email = email,
                        FullName = FullName,
                        AvatarUrl = picture,
                    };
                    var createdUser = await _userManager.CreateAsync(user);
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
                                    Token = _tokenService.CreateToken(user, roles)
                                }
                            );
                        }
                        return StatusCode(500, roleResult.Errors);
                    }
                    return StatusCode(500, createdUser.Errors);
                }
                var roless = await _userManager.GetRolesAsync(user);
                return Ok(
                                new UserReturn
                                {
                                    UserName = user.UserName,
                                    Email = user.Email,
                                    FullName = user.FullName,
                                    Token = _tokenService.CreateToken(user, roless)
                                }
                            );
            }
            return BadRequest("Đăng nhập thất bại.");
        }
    }
}
