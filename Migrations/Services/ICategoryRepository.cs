using OnlineLearningSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLearningSystem.Services
{
    public interface ICategoryRepository
    {
        ICollection<Category> GetCategories();
        Category GetCategory(int categoryId);
        bool CategoryExists(int categoryId);
        bool IsDuplicateCategoryTitle(int categoryId, string categoryTitle);
        bool CreateCategory(Category category);
        bool AddACategoryToACourse(Course course, Category category);
        bool UpdateCategory(Category category);
        bool DeleteCategory(Category category);
        bool DeleteACategoryToACourse(Course course, Category category);
        ICollection<Category> GetAllCategoriesOfCourse(int courseId);
        bool Save();

    }
}
