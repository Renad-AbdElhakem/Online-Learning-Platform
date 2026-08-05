namespace AssignmentService.Dtos
{
    public class SubmittedAssignmentDto
    {
        public IFormFile SubmissionFile { get; set; } = null!;
        public Guid AssignmentId { get; set; }
        public Guid StudentId { get; set; }
    }
}
