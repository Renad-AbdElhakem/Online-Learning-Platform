namespace InstructorService.Dtos
{
    public class CreateInstructorDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Major { get; set; }
        public string Bio { get; set; }

        public DateOnly StartDate { get; set; }
    }
}
