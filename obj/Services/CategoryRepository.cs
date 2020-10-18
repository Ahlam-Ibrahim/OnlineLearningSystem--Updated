using OnlineLearningSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLearningSystem.Services
{
    public class CategoryRepository : ICategoryRepository
    {
        private ApplicationDbContext _categoryContext;

        public CategoryRepository(ApplicationDbContext categoryContext)
        {
            _categoryContext = categoryContext;
        }

        public bool AddACategoryToACourse(Course course, Category category)
        {
            CourseCategory courseCategory = new CourseCategory
            {
                Course = course,
                CourseID = course.Id,
                Category = category,
                CategoryID = category.Id
            };
            _categoryContext.CourseCategories.Add(courseCategory);
            return Save();
        }

        public bool CategoryExists(int categoryId)
        {
            return _categoryContext.Categories.Any(c => c.Id == categoryId);
        }

        public bool CreateCategory(Category category)
        {
            _categoryContext.Add(category);
            return Save();
        }

        public bool DeleteACategoryToACourse(Course course, Category category)
        {
            CourseCategory courseCategory = new CourseCategory
            {
                Course = course,
                CourseID = course.Id,
                Category = category,
                CategoryID = category.Id
            };
            _categoryContext.CourseCategories.Remove(courseCategory);
            return Save();
        }

        public bool DeleteCategory(Category category)
        {
            _categoryContext.Remove(category);
            return Save();
        }

        public ICollection<Category> GetAllCategoriesOfCourse(int courseId)
        {
            //many-to-many
            return _categoryContext.CourseCategories.Where(c => c.CourseID == courseId)
                .Select(c => c.Category).ToList();
        }
        public ICollection<Category> GetCategories()
        {
            return _categoryContext.Categories.OrderBy(c => c.Title).ToList();
        }

        public Category GetCategory(int categoryId)
        {
            return _categoryContext.Categories.Where(c => c.Id == categoryId).FirstOrDefault();
        }
        public bool IsDuplicateCategoryTitle(int categoryId, string categoryTitle)
        {
            var course = _categoryContext.Categories.Where(c => c.Title.Trim().ToUpper()
                        == categoryTitle.Trim().ToUpper() && c.Id != categoryId).FirstOrDefault();

            return course == null ? false : true;
        }

        public bool Save()
        {
            var saved = _categoryContext.SaveChanges();
            return saved >= 0 ? true : false;
        }

        public bool UpdateCategory(Category category)
        {
            _categoryContext.Update(category);
            return Save();
        }
    }
}
