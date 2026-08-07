namespace AuthService.Dtos
{
    public class RegisterationNewUserDto
    {
        public string Email { get; set; }
        public bool IsEmailConfirmed { get; set; } = false;
        public string Password { get; set; }
        public int RoleId { get; set; }
    }
}
