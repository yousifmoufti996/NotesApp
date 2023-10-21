namespace notesApp.API.Models.DomainModels
{
    public class Note
    {
        public Guid Id{ get; set; }
        public string? Title { get; set; }   
        public string? Description { get; set; }   
        public DateTime? DateCreated { get; set; }  
        public DateTime? DateDeleted { get; set; }  
        public DateTime? DateUpdated { get; set; }
        public string UserId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }

    }
}
