namespace AssignmentService.Model
{
    public class StudentAssignment
    {
        public Guid Id { get; set; }
       
        public string? SubmissionFileUrl { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public bool IsSubmitted { get; set; } = false;
        public bool IsLater { get; set; } 

        public decimal? Grade { get; set; }
        public string? Feedback { get; set; }

        public Guid StudentId { get; set; }      
        public Guid AssignmentId { get; set; }  
        public Assignment Assignment { get; set; } = null!;
    }
}
