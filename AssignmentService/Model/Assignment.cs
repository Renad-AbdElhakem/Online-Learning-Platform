namespace AssignmentService.Model
{
    public class Assignment
    {
        public Guid Id { get; set; }

        public string AssignmentPath { get; set; } = null!;
        public string AssignmentName { get; set; }
        public string? Notes { get; set; }
        public int Grade { get; set; }
        public DateOnly UploadedAt { get; set; }
        public DateTime ?ModifiedDate { get; set; }
        public DateTime StartAt { get; set; }    
        public int DurationInDays { get; set; }    
        public DateTime EndAt { get; set; }       
        public bool IsVisible { get; set; }
        public Guid GroupId { get; set; }

        public ICollection<StudentAssignment> ?StudentAssignments { get; set; } = new List<StudentAssignment>();
    }
}
