namespace CourseService.Dtos
{
    public class CreateNewInstructorCourse
    {
       
        public bool IsActive { get; set; } = true;
        public string? status { get; set; }

        public Guid InstructorId { get; set; }

        public Guid CourseId { get; set; }
    }
}
