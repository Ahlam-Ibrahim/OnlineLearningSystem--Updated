﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using OnlineLearningSystem.Models;
using System.Text;
using OnlineLearningSystem.Services;
using OnlineLearningSystem.Dtos;

namespace OnlineLearningSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUserController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _singInManager;
        private readonly ApplicationSettings _appSettings;
        private ICourseRepository _courseRepository;
        public ApplicationUserController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<ApplicationSettings> appSettings,
            ICourseRepository courseRepository)
        {
            _userManager = userManager;
            _singInManager = signInManager;
            _appSettings = appSettings.Value;
            _courseRepository = courseRepository;
        }

        [HttpPost]
        [Route("register")]
        //POST : /api/ApplicationUser/register
        public async Task<Object> PostApplicationUser(ApplicationUserModel model)
        {
            //The role assigned when a new member signs up
            model.Role = "Admin";
            var applicationUser = new ApplicationUser() {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName
            };

            try
            {
                var result = await _userManager.CreateAsync(applicationUser, model.Password);
                await _userManager.AddToRoleAsync(applicationUser, model.Role);
                return Ok(result);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        [Route("login")]
        //POST : /api/ApplicationUser/login
        public async Task<IActionResult> Login(LoginModel model)
        {

            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                //Get role assigned to the user
                var role = await _userManager.GetRolesAsync(user);
                IdentityOptions _options = new IdentityOptions();

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("UserID",user.Id.ToString()),
                        new Claim(_options.ClaimsIdentity.RoleClaimType,role.FirstOrDefault()),
                    }),
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);
                return Ok(new { token });
            }
            else
                return BadRequest(new { message = "Username or password is incorrect." });
        }

        [HttpGet]
        [Route("profile")]
        //GET: /api/content/profile
        public async Task<Object> GetUserProfile()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            return new
            {
                user.FullName,
                user.PhoneNumber,
                user.Email,
                user.UserName
            };
        }
        //ApplicationUser - Student Courses => MyCourses

        //[HttpGet]
        ////GET : /api/ApplicationUser
        //public async Task<IActionResult> GetMyCourses()
        //{
        //    ClaimsPrincipal currentUser = this.User;
        //    var currentUserName = "lll";
        //    if (currentUser == null)
        //    {
        //        currentUserName = "kkk";
        //        currentUserName = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
        //    }
        //    //ApplicationUser user = await _userManager.FindByNameAsync(currentUserName);

        //    //var courses = _courseRepository.GetMyCourses(currentUserName);

        //    //if (!ModelState.IsValid)
        //    //    return BadRequest();

        //    //var coursesDto = new List<CourseDto>();
        //    //foreach (var course in courses)
        //    //{
        //    //    coursesDto.Add(new CourseDto
        //    //    {
        //    //        Id = course.Id,
        //    //        Title = course.Title,
        //    //        Description = course.Description,
        //    //        Duration = course.Duration,
        //    //        DateCreated = course.DateCreated
        //    //    });
        //    //}
        //    return Ok(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);

        //}
    }
}