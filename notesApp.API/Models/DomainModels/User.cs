using Microsoft.AspNetCore.Identity;
using notesApp.API.Models.DomainModels;
namespace notesApp.API.Utils;

public class User : IdentityUser 
{
    
    public string? Address { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }


}
