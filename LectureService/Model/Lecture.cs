using Content_Service.Model;
using Microsoft.AspNetCore.Http;

namespace LectureService.Model
{
    public class Lecture
    {
        public Guid Id { get; set; }

        public string VideoLecture { get; set; } = string.Empty;

        public string LectureTitle { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.Now;

        public DateTime? ModifiedAt { get; set; }

        public Guid GroupId { get; set; }

        public ICollection<Material> ?Materials { get; set; }= new List<Material>();
    }
}
