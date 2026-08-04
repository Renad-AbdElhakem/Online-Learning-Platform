namespace Content_Service.Dtos
{
    public class AddNewLectureMaterialDto
    {
        public IFormFile LectureMaterial { get; set; }

        public string MaterialName { get; set; }

        public string? Description { get; set; }
        public bool IsVisible { get; set; } = true;

        public Guid LectureId { get; set; }
    }
}
