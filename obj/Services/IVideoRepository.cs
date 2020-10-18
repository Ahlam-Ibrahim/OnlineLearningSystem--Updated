using OnlineLearningSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLearningSystem.Services
{
    public interface IVideoRepository
    {
        ICollection<Video> GetAllVideosFromSection(int sectionId);
        Video GetVideo(int sectionId);
        bool VideoExists(int id);
        bool CreateVideo(Video section);
        bool UpdateVideoOfASection(Video section);
        bool DeleteVideo(Video section);

        //ICollection<Section> GetAllCoursesOfCategory(int categoryId);
        //ICollection<Section> GetAllSectionOfACourse(int courseId);
        bool Save();
    }
}
