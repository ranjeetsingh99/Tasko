using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Tasko.Models;

namespace Tasko.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        private readonly IDataProtector _notesProtector;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,IDataProtectionProvider dataProtection)
            : base(options)
        {
            _notesProtector = dataProtection.CreateProtector("Todo.Notes.v1");
        }
        public DbSet<TodoItem> Todos => Set<TodoItem>();
        public DbSet<Category> Categories => Set<Category>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            //Category..............
            b.Entity<Category>(e =>
            {
                
                e.HasOne(c=> c.Owner)
                .WithMany(u=> u.Categories)
                .HasForeignKey(c => c.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            //Todos...........
            b.Entity<TodoItem>(e =>
            {
                e.ToTable(t => t.HasCheckConstraint("CK_Todos_Priority", "[Priority] IN (0,1,2)"));

                e.Property(t => t.Notes)
                .HasConversion(new EncryptedStringConverter(_notesProtector));

                e.HasIndex(t => new { t.OwnerId, t.IsDone, t.DueDate });

                e.HasQueryFilter(t => !t.IsDeleted);
            });


        }
    }


    


}
