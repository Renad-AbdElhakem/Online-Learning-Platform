namespace AuthService.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public bool IsEmailConfirmed { get; set; } = false;
        public string HashedPassword { get; set; }
        public DateTime LoggedInAt { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
