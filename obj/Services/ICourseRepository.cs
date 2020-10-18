using OnlineLearningSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLearningSystem.Services
{
    public interface ICourseRepository
    {
        ICollection<Course> GetCourses();
        Course GetCourse(int courseId);
        bool CourseExists(int id);
        bool IsDuplicateCourseName(int id, string courseName);
        bool CreateCourse(Course course);
        bool CreateSectionOfACourse(int courseId, Section section);
        bool UpdateCourse(Course course);
        bool UpdateSectionOfACourse(Course course, Section section, string newSection);
        bool DeleteCourse(Course course);
        bool DeleteSectionOfACourse(Section section, Course course);
        ICollection<Course> GetAllCoursesOfCategory(int categoryId);
        ICollection<Section> GetAllSectionOfACourse(int courseId);
        bool Save();

    }
}
