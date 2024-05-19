﻿using Assignment_Server.Interfaces;
using Assignment_Server.Models;
using Assignment_Server.Models.DTO.User;
using Assignment_Server.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    }
}
