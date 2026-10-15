using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Tasko.Models;

namespace Tasko.Data
{
    public class ApplicationDbContext : IdentityDbContext,IDataProtectionKeyContext
    {
        private readonly IDataProtector _notesProtector;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDataProtectionProvider dataProtection)
            : base(options)
        {
            _notesProtector = dataProtection.CreateProtector("Todo.Notes.v1");
        }
        public DbSet<TodoItem> Todos => Set<TodoItem>();
        public DbSet<Category> Categories => Set<Category>();

        public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            //Category..............
            b.Entity<Category>(e =>
            {

                e.HasOne(c => c.Owner)
                .WithMany(u => u.Categories)
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

            //subtask...........
            b.Entity<SubTask>(e =>
            {
                e.HasIndex(s => new { s.TodoItemId, s.SortOrder });
                e.HasOne(s => s.TodoItem)
                .WithMany(t => t.SubTasks)
                .HasForeignKey(s => s.TodoItemId)
                .OnDelete(DeleteBehavior.Cascade);
                e.HasQueryFilter(s => !s.TodoItem.IsDeleted);
            });



        }
        //audit rules.................
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ApplyAuditRules();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken ct = default)
        {
            ApplyAuditRules();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, ct);
        }

        private void ApplyAuditRules()
        {
            var now = DateTime.UtcNow;
            foreach (var entry in ChangeTracker.Entries<TodoItem>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = now;
                }
                else continue;

                if (entry.Entity.IsDone && entry.Entity.CompletedAt == null)
                    entry.Entity.CompletedAt = now;
                else if (!entry.Entity.IsDone)
                    entry.Entity.CompletedAt = null;
            }
        }
    }
}
