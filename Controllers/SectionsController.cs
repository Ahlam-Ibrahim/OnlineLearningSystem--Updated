using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningSystem.Dtos;
using OnlineLearningSystem.Models;
using OnlineLearningSystem.Services;

namespace OnlineLearningSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SectionsController : Controller
    {
        private ICourseRepository _courseRepository;
        private ISectionRepository _sectionRepository;
        private IVideoRepository _videoRepository;
        private IEnrollmentRepository _enrollmentRepository;
        private UserManager<ApplicationUser> _userManager;

        public SectionsController(ICourseRepository courseRepository,
            ISectionRepository sectionRepository,
            IEnrollmentRepository enrollmentRepository,
            UserManager<ApplicationUser> userManager,
            IVideoRepository videoRepository)
        {
            _courseRepository = courseRepository;
            _sectionRepository = sectionRepository;
            _userManager = userManager;
            _enrollmentRepository = enrollmentRepository;
            _videoRepository = videoRepository;
        }

        //This request returns the content/sections of the course, unlocking
        //the course content is allowed for students whom orders were approved
        //by the admin.
        //admin and mentor can access the course sections w/o restrictions.

        // GET api/sections/courseId
        [HttpGet("{courseId}")]
        [Authorize(Roles = "Admin, Mentor, Student", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Object> GetAllSectionOfACourse(int courseId)
        {

            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            var userRole = await _userManager.GetRolesAsync(user);

            var myCourses = _enrollmentRepository.GetMyCourses(userId).ToList();
            bool approvedForStudent = false;

            //First: retrieve the course's sections
            var sections = new List<Section>();
            sections = (List<Section>)_courseRepository.GetAllSectionOfACourse(courseId);
            var sectionsDto = new List<SectionDto>();
            foreach (var section in sections)
            {
                sectionsDto.Add(new SectionDto
                {
                    Id = section.Id,
                    Title = section.Title,
                    Videos = section.Videos
                });
            }
            if (userRole[0].Equals("Admin") || userRole[0].Equals("Mentor"))
            {
                return Ok(sectionsDto);
            }

            if (userRole[0].Equals("Student"))
            {

                foreach (var course in myCourses)
                //all the courses in myCourses are approved by the admin
                {
                    if (courseId == course.Id)
                    {
                        approvedForStudent = true;
                        break;
                    }
                }
                if (approvedForStudent)
                {
                    return Ok(sectionsDto);
                }
                else //if the course is not approved
                {
                    return Ok("You can't view the content of this course," +
                        "order the course, or follow your pre-existing order");
                }


            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //otherwise
            return Ok("You are not authorized to view this page!");
        }

        // GET api/sections/courseId/sectionId
        [HttpGet("{courseId}/{sectionId}", Name = "GetSection")]
        [Authorize(Roles = "Admin, Mentor, Student", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Object> GetSection(int sectionId, int courseId)
        {

            if (!_sectionRepository.SectionExists(sectionId))
                return NotFound();
            if (!ModelState.IsValid)
                return BadRequest();

            var section = new Section();

            section = _sectionRepository.GetSection(sectionId);

            if (section == null)
                return BadRequest();

            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            var userRole = await _userManager.GetRolesAsync(user);

            var myCourses = _enrollmentRepository.GetMyCourses(userId).ToList();
            bool approvedForStudent = false;

            var sectionDto = new SectionDto()
            {
                Id = section.Id,
                Title = section.Title,
                Videos = section.Videos
            };

            if (userRole[0].Equals("Admin") || userRole[0].Equals("Mentor"))
            {
                return Ok(sectionDto);
            }

            if (userRole[0].Equals("Student"))
            {

                foreach (var course in myCourses)
                //all the courses in myCourses are approved by the admin
                {
                    if (courseId == course.Id)
                    {
                        approvedForStudent = true;
                        break;
                    }
                }
                if (approvedForStudent)
                {
                    return Ok(sectionDto);
                }
                else //if the course is not approved
                {
                    return Ok("You can't view the content of this course," +
                        "order the course, or follow your pre-existing order");
                }


            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //otherwise
            return Ok("Please Sign Up!");
        }

        // POST api/sections/courseId
        [HttpPost("{courseId}")]
        [Authorize(Roles = "Admin,Mentor", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult CreateSectionOfACourse([FromBody] Section section, int courseId)
        {
            if (section == null)
                return BadRequest();

            if (_courseRepository.GetCourse(courseId) == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_courseRepository.CreateSectionOfACourse(courseId, section))
            {
                ModelState.AddModelError("", $"Something went wrong!");
                return BadRequest(ModelState);
            }

            return CreatedAtRoute("GetSection", new { courseId = courseId, sectionId = section.Id }, section);
        }

        // DELETE api/sections/courseId/sectionId
        [HttpDelete("{courseId}/{sectionId}")]
        [Authorize(Roles = "Admin,Mentor", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult DeleteSection(int courseId, int sectionId)
        {
            if (!_courseRepository.CourseExists(courseId))
                return NotFound();

            if (!_sectionRepository.SectionExists(sectionId))
                return NotFound();

            var section = _sectionRepository.GetSection(sectionId);
            var course = _courseRepository.GetCourse(courseId);

            if (_videoRepository.GetAllVideosFromSection(sectionId).Count() > 0)
            {
                var videos = _videoRepository.GetAllVideosFromSection(sectionId);
                foreach (var video in videos)
                {
                    if (!_videoRepository.DeleteVideo(video))
                        return BadRequest(ModelState);
                }
            }

            if (!_courseRepository.DeleteSectionOfACourse(section, course))
                ModelState.AddModelError("", $"Something went wrong!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return NoContent();

        }


        // PUT api/courses/sections/courseId/sectionId
        [HttpPut("{courseId}/{sectionId}")]
        [Authorize(Roles = "Admin,Mentor", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult UpdateSectionOfACourse(int courseId, int sectionId,
            [FromBody] string newSection)
        {

            if (!_courseRepository.CourseExists(courseId))
                return NotFound();

            if (!_sectionRepository.SectionExists(sectionId))
                return NotFound();

            var section = _sectionRepository.GetSection(sectionId);
            var course = _courseRepository.GetCourse(courseId);

            if (!_courseRepository.UpdateSectionOfACourse(course, section, newSection))
                ModelState.AddModelError("", $"Something went wrong!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return NoContent();

        }
    }
}