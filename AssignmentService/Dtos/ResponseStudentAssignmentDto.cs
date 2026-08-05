namespace AssignmentService.Dtos
{
    public class ResponseStudentAssignmentDto
    {
        public string? SubmissionFileUrl { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public bool IsSubmitted { get; set; } = false;
        public bool IsLater { get; set; }

        public decimal? Grade { get; set; }
        public string? Feedback { get; set; }
    }
}
