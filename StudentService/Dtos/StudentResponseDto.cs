namespace StudentService.Dtos
{
    public class StudentResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
    }
}
