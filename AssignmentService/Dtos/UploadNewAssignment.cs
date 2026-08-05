namespace AssignmentService.Dtos
{
    public class UploadNewAssignment
    {
        public IFormFile AssignmentFile { get; set; } = null!;
       
        public string AssignmentName { get; set; } = null!;

        public Guid GroupId { get; set; }
        public string? Notes { get; set; }

        public int Grade { get; set; }

        public DateTime StartAt { get; set; }

        public int DurationInDays { get; set; }

        public bool IsVisible { get; set; }


    }
}
