using LectureService.Model;

namespace Content_Service.Model
{
    public class Material
    {
        public Guid Id { get; set; }

        public string LectureMaterialPath { get; set; } = string.Empty;

        public string MaterialName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.Now;

        public DateTime? ModifiedAt { get; set; }

        public bool IsVisible { get; set; } = true;

        public Guid LectureId { get; set; }
        public Lecture Lecture { get; set; } 

    }
}
