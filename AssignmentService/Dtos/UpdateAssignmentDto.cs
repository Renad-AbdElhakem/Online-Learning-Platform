namespace AssignmentService.Dtos
{
    public class UpdateAssignmentDto
    {
        public IFormFile? AssignmentFile { get; set; }
        public string? AssignmentName { get; set; }

        public string? Notes { get; set; }

        public int? Grade { get; set; }

        public DateTime? StartAt { get; set; }

        public int? DurationInDays { get; set; }

     

        public Guid? GroupId { get; set; }

    }
}
