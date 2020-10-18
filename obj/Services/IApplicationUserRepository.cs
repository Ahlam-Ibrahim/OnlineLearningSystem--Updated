using OnlineLearningSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLearningSystem.Services
{
    public interface IApplicationUserRepository
    {
        //ICollection<Course> GetCourses();
        ApplicationUser GetUserInfo(int courseId);
        //bool CourseExists(int id);
        //bool IsDuplicateCourseName(int id, string courseName);
        //bool CreateCourse(Course course);
        //bool CreateSectionOfACourse(int courseId, Section section);
        //ICollection<Course> GetMyCourses(string userName);
        //bool UpdateCourse(Course course);
        //bool UpdateSectionOfACourse(Course course, Section section, string newSection);
        //bool DeleteCourse(Course course);
        //bool DeleteSectionOfACourse(Section section, Course course);
        //ICollection<Course> GetAllCoursesOfCategory(int categoryId);
        //ICollection<Section> GetAllSectionOfACourse(int courseId);
        //bool Save();

    }
}
