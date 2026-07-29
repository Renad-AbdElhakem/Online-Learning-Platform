namespace InstructorService.Dtos
{
    public class InstructorDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Major { get; set; }
        public string Bio { get; set; }

        public string Status { get; set; }

        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}
