using OnlineLearningSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLearningSystem.Services
{
    public interface ISectionRepository
    {
        ICollection<Section> GetSections();
        Section GetSection(int sectionId);
        bool SectionExists(int id);
        bool IsDuplicateSectionTitle(int id, string sectionTitle);
        bool CreateSection(Section section);
        bool UpdateSection(Section section);
        bool DeleteSection(Section section);

        //ICollection<Section> GetAllCoursesOfCategory(int categoryId);
        //ICollection<Section> GetAllSectionOfACourse(int courseId);
        bool Save();

    }
}
