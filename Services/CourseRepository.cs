using OnlineLearningSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;

namespace OnlineLearningSystem.Services
{
    public class CourseRepository : ICourseRepository
    {
        private ApplicationDbContext _courseContext;
        public CourseRepository(ApplicationDbContext courseContext)
        {
            _courseContext = courseContext;
        }

        public bool CourseExists(int id)
        {
            return _courseContext.Courses.Any(c => c.Id == id);
        }

        public bool CreateCourse(Course course)
        {
            _courseContext.Add(course);
            return Save();
        }

        public bool CreateSectionOfACourse(int courseId, Section section)
        {
            var course = GetCourse(courseId);

            course.Sections = new List<Section>();
            course.Sections.Add(section);

            section.Course = course;
            _courseContext.Sections.Add(section);

            return Save();
        }

        public bool DeleteCourse(Course country)
        {
            _courseContext.Remove(country);
            return Save();
        }

        public bool DeleteSectionOfACourse(Section section, Course course)
        {
            _courseContext.Sections.Remove(section);
            return Save();
        }

        public bool UpdateSectionOfACourse(Course course, Section section, string newSection)
        {
            section.Title = newSection;

            //after adding videos this method will be updated 

            _courseContext.Sections.Update(section);

            return Save();
        }

        public ICollection<Course> GetAllCoursesOfCategory(int categoryId)
        {
            return _courseContext.CourseCategories.Where(c => c.CategoryID == categoryId)
                          .Select(c => c.Course).ToList();
        }

        public ICollection<Section> GetAllSectionOfACourse(int courseId)
        {
            return _courseContext.Sections.Where(c => c.Course.Id == courseId).ToList();
        }

        public Course GetCourse(int courseId)
        {
            return _courseContext.Courses.Where(c => c.Id == courseId).FirstOrDefault();
        }

        public ICollection<Course> GetCourses()
        {
            return _courseContext.Courses.OrderBy(c => c.Title).ToList();
        }

        public bool IsDuplicateCourseName(int id, string counrseName)
        {
            var course = _courseContext.Courses.Where(c => c.Title.Trim().ToUpper()
            == counrseName.Trim().ToUpper() && c.Id != id).FirstOrDefault();

            return course == null ? false : true;
        }

        public bool Save()
        {
            var saved = _courseContext.SaveChanges();
            return saved >= 0 ? true : false;
        }

        public bool UpdateCourse(Course course)
        {
            _courseContext.Update(course);
            return Save();
        }

    }
}
