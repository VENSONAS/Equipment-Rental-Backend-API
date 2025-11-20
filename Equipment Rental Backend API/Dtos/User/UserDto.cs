namespace Equipment_Rental_Backend_API.Dtos.User
{
    public class UserDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Role { get; set; } 
        public DateTime CreatedAt { get; set; }
    }

    public class CreateUserDto
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
    }

    public class UpdateUserDto
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
    }
}
