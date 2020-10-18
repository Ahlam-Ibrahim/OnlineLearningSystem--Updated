using OnlineLearningSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLearningSystem.Services
{
    public class SectionRepository : ISectionRepository
    {
        private ApplicationDbContext _sectionContext;

        public SectionRepository(ApplicationDbContext sectionContext)
        {
            _sectionContext = sectionContext;
        }
        public bool CreateSection(Section section)
        {
            _sectionContext.Add(section);
            return Save();
        }

        public bool DeleteSection(Section section)
        {
            _sectionContext.Remove(section);
            return Save();
        }

        public Section GetSection(int sectionId)
        {
            return _sectionContext.Sections.Where(c => c.Id == sectionId).FirstOrDefault();
        }

        public ICollection<Section> GetSections()
        {
            return _sectionContext.Sections.OrderBy(c => c.Title).ToList();
        }

        public bool IsDuplicateSectionTitle(int id, string sectionTitle)
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            var saved = _sectionContext.SaveChanges();
            return saved >= 0 ? true : false;
        }

        public bool SectionExists(int id)
        {
            return _sectionContext.Sections.Any(c => c.Id == id);
        }

        public bool UpdateSection(Section section)
        {
            _sectionContext.Update(section);
            return Save();
        }
    }
}
