using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
        private UserManager<ApplicationUser> _userManager;

        public CoursesController(ICourseRepository courseRepository, 
            ISectionRepository sectionRepository,
            UserManager<ApplicationUser> userManager)
        {
            _courseRepository = courseRepository;
            _sectionRepository = sectionRepository;
            _userManager = userManager;
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
                    DateCreated = course.DateCreated.Date,
                    Location = course.Location
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

            var courseDto = new CourseDto {
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
                    DateCreated = course.DateCreated,
                    Location = course.Location,
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
            if ( course == null )
                return BadRequest();

            if( courseId != course.Id )
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

            if ( _courseRepository.GetAllSectionOfACourse(courseId).Count() > 0) {
                var sections = _courseRepository.GetAllSectionOfACourse(courseId);
                foreach(var section in sections)
                {
                  if(! _sectionRepository.DeleteSection(section) )
                         return BadRequest(ModelState);
                }
            }
            if (!_courseRepository.DeleteCourse(course))
                ModelState.AddModelError("", $"Something went wrong!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return NoContent();
        }

        // Sections -- Controlling the section(s) sonnected to a course

        //// GET api/courses/sections/courseId
        //[HttpGet("sections/{courseId}")]
        //public ActionResult GetAllSectionOfACourse(int courseId)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);
        //    //only if the course status was: Approved
        //    var course = _courseRepository.GetCourse(courseId);
        //    var sections = new List<Section>();
        //    int courseStatus = (int) course.Status;
        //    if (courseStatus == 1) //Approved
        //    {
        //        sections = (List<Section>)_courseRepository.GetAllSectionOfACourse(courseId);
        //        var sectionsDto = new List<SectionDto>();
        //        foreach (var section in sections)
        //        {
        //            sectionsDto.Add(new SectionDto
        //            {
        //                Id = section.Id,
        //                Title = section.Title,
        //                Videos = section.Videos
        //            });
        //        }

        //        return Ok(sectionsDto);
        //    }
        //    else if(courseStatus == 2) //waiting for payment
        //    {
        //        return Ok("You need to pay before viewing/attending this course, for more info click here: ");
        //    }
        //    else if(courseStatus == 0) //ordered
        //    {
        //        return Ok("Your order is under proccessing. Please wait.");
        //    }
        //    else //if the user didn't order yet - Default
        //    {
        //        return Ok("You have to order this course to be able to view/attend it");
        //    }
        //}

        //// GET api/courses/section/sectionId
        //[HttpGet("section/{sectionId}", Name = "GetSection")]
        //public ActionResult GetSection(int sectionId)
        //{
        //    if (!_sectionRepository.SectionExists(sectionId))
        //        return NotFound();
        //    if (!ModelState.IsValid)
        //        return BadRequest();

        //    var section = _sectionRepository.GetSection(sectionId);
        //    int courseStatus = (int)section.Course.Status;
     
        //    if (courseStatus == 2) //Approved
        //    {
        //        var sectionDto = new SectionDto
        //        {
        //            Id = section.Id,
        //            Title = section.Title,
        //            Videos = section.Videos
        //        };

        //        return Ok(sectionDto);
        //    }
        //    else //otherwise - the user shouldn't be able to reach this method getSection
        //    //unlike the one above, that's why it doen;t contain detailed error msgs
        //    {
        //        return Ok("You are not allowed to view this course's section");
        //    }
        //}

        // POST api/courses/sections/courseId
        [HttpPost("sections/{courseId}")]
        [Authorize(Roles = "Admin,Mentor", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult CreateSectionOfACourse(int courseId, [FromBody] Section section)
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

            return CreatedAtRoute("GetSection", new { sectionId = section.Id }, section);

        }

        // DELETE api/courses/sections/courseId/sectionId
        [HttpDelete("sections/{courseId}/{sectionId}")]
        [Authorize(Roles = "Admin,Mentor", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult DeleteSectionOfACourse(int courseId, int sectionId)
        {
            if (!_courseRepository.CourseExists(courseId))
                return NotFound();

            if (!_sectionRepository.SectionExists(sectionId))
                return NotFound();

            var section = _sectionRepository.GetSection(sectionId);
            var course = _courseRepository.GetCourse(courseId);

            if (!_courseRepository.DeleteSectionOfACourse(section, course))
                ModelState.AddModelError("", $"Something went wrong!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return NoContent();

        }


        //after adding videos this method will be updated 
        // PUT api/courses/sections/courseId/sectionId
        [HttpPut("sections/{courseId}/{sectionId}")]
        [Authorize(Roles = "Admin,Mentor", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult UpdateSectionOfACourse(int courseId, int sectionId, [FromBody] string newSection)
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

        // MyCourses Section - courses for a particular student

        [HttpGet("myCourses")]
        [Authorize(Roles= "Student", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //GET: /api/content/profile
        public async Task<Object> GetMyCourses()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            var myCourses = _courseRepository.GetMyCourses(userId).ToList();
            //only the approved courses will be viewable
            return Ok(myCourses);
        }

        // POST api/categories/categoryId/courseId
        [HttpPost("add/{courseId}")]
        [Authorize(Roles = "Student", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Object> AddACourseToStudent(int courseId)
        {
            if (courseId == null)
                return BadRequest();

            var course = _courseRepository.GetCourse(courseId);
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            
            if (course == null)
                return BadRequest();

            if (user == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_courseRepository.AddCourseToStudent(course, user))
            {
                ModelState.AddModelError("", $"Something went wrong!");
                return BadRequest(ModelState);
            }

            return CreatedAtRoute("GetCourse", new { courseId = course.Id }, course);

        }

        // POST api/courses/order/courseId
        [HttpPost("order/{courseId}")]
        [Authorize(Roles = "Student", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Object> OrderACourse(int courseId)
        {
            if (courseId == null)
                return BadRequest();

            var course = _courseRepository.GetCourse(courseId);
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);

            if (course == null)
                return BadRequest();

            if (user == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_courseRepository.OrderACourse(course, user))
            {
                ModelState.AddModelError("", $"Something went wrong!");
                return BadRequest(ModelState);
            }

            return CreatedAtRoute("GetCourse", new { courseId = course.Id }, course);

        }


    }
}
