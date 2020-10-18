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
    public class CoursesController : Controller
    {
        private ICourseRepository _courseRepository;
        private ISectionRepository _sectionRepository;
        private IVideoRepository _videoRepository;
        private UserManager<ApplicationUser> _userManager;

        public CoursesController(ICourseRepository courseRepository,
            ISectionRepository sectionRepository,
            UserManager<ApplicationUser> userManager,
            IVideoRepository videoRepository)
        {
            _courseRepository = courseRepository;
            _sectionRepository = sectionRepository;
            _userManager = userManager;
            _videoRepository = videoRepository;
        }

        // GET api/courses
        [HttpGet]
        public ActionResult GetCourses()
        {
            var courses = _courseRepository.GetCourses().ToList();

            if (!ModelState.IsValid)
                return BadRequest();

            var coursesDto = new List<CourseDto>();
            foreach (var course in courses)
            {
                coursesDto.Add(new CourseDto
                {
                    Id = course.Id,
                    Title = course.Title,
                    Description = course.Description,
                    Duration = course.Duration,
                    DateCreated = course.DateCreated.Date
                });
            }
            return Ok(coursesDto);
        }
        // GET api/courses/courseId
        [HttpGet("{courseId}", Name = "GetCourse")]
        public ActionResult GetCourse(int courseId)
        {
            if (!_courseRepository.CourseExists(courseId))
                return NotFound();

            var course = _courseRepository.GetCourse(courseId);

            if (!ModelState.IsValid)
                return BadRequest();

            var courseDto = new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Duration = course.Duration,
                DateCreated = course.DateCreated.Date
            };

            return Ok(courseDto);
        }

        // GET api/courses/categories/categoryId
        [HttpGet("categories/{categoryId}")]
        public ActionResult GetAllCoursesOfCategory(int categoryId)
        {

            var courses = _courseRepository.GetAllCoursesOfCategory(categoryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var CoursesDto = new List<CourseDto>();
            foreach (var course in courses)
            {
                CoursesDto.Add(new CourseDto
                {
                    Id = course.Id,
                    Title = course.Title,
                    Description = course.Description,
                    Duration = course.Duration,
                    DateCreated = course.DateCreated
                });
            }

            return Ok(CoursesDto);
        }

        // POST api/courses
        [HttpPost]
        [Authorize(Roles = "Admin,Mentor", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult CreateCourse([FromBody] Course course)
        {
            course.DateCreated = DateTime.Now.Date;

            if (course == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_courseRepository.CreateCourse(course))
                ModelState.AddModelError("", $"Something went wrong!");

            return CreatedAtRoute("GetCourse", new { courseId = course.Id }, course);

        }

        // POST api/courses/courseId
        [HttpPut("{courseId}")]
        [Authorize(Roles = "Admin,Mentor", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult UpdateCourse(int courseId, [FromBody] Course course)
        {
            if (course == null)
                return BadRequest();

            if (courseId != course.Id)
                return BadRequest();

            if (!_courseRepository.CourseExists(courseId))
                return NotFound();

            if (!_courseRepository.UpdateCourse(course))
                ModelState.AddModelError("", $"Something went wrong!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return NoContent();

        }


        // DELETE api/courses/courseId
        [HttpDelete("{courseId}")]
        [Authorize(Roles = "Admin,Mentor", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult DeleteCourse(int courseId)
        {

            if (!_courseRepository.CourseExists(courseId))
                return NotFound();

            var course = _courseRepository.GetCourse(courseId);

            //There is a conflict with the section table 
            //if the course has sections delete them too
            //The conflict arise in one-to-many relationshsips

            if (_courseRepository.GetAllSectionOfACourse(courseId).Count() > 0)
            {
                var sections = _courseRepository.GetAllSectionOfACourse(courseId);
                foreach (var section in sections)
                {
                    if (!_sectionRepository.DeleteSection(section))
                        return BadRequest(ModelState);
                }
            }
            if (!_courseRepository.DeleteCourse(course))
                ModelState.AddModelError("", $"Something went wrong!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return NoContent();
        }

    }
}
