using Microsoft.EntityFrameworkCore;
using notesApp.API.Models.DomainModels;
using notesApp.API.Utils;

namespace notesApp.API.Data
{
    public class NotesDBCon : DbContext
    {
        public NotesDBCon(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring
       (DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseInMemoryDatabase(databaseName: "notesdb").AddInterceptors(new SoftDeleteInterceptor());
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasQueryFilter(user => !user.IsDeleted);
            modelBuilder.Entity<Note>().HasQueryFilter(note => !note.IsDeleted);

            // Define shadow properties for soft delete
            modelBuilder.Entity<User>().Property<bool>("IsDeleted");
            modelBuilder.Entity<User>().Property<DateTimeOffset?>("DeletedAt");
            modelBuilder.Entity<User>().Property<DateTime?>("DateDeleted");

            modelBuilder.Entity<Note>().Property<bool>("IsDeleted");
            modelBuilder.Entity<Note>().Property<DateTimeOffset?>("DeletedAt");
            modelBuilder.Entity<Note>().Property<DateTime?>("DateDeleted");
        }

        public DbSet <Note> Notes { get; set; }
        public DbSet<User>Users { get; set; }

      
        

    }
}
