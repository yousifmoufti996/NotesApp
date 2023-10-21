namespace notesApp.API.Models.DataTransferObject
{
    public class LoginDto
    {
    
        public string Email { get; set; }
        public string Password { get; set; }


    }

    public class RegisterDto
    {
        public string Email { get; set; }
        


        public string? UserName { get; set; }
        public string? Address{ get; set; }
        public string Password { get; set; }
        public string? FullName { get; set; }

    }
}