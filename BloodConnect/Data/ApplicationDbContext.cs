using BloodConnect.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BloodConnect.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<BloodRequest> BloodRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<BloodRequest>()
                .HasOne(br => br.Donor)
                .WithMany()
                .HasForeignKey(br => br.DonorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<BloodRequest>()
                .HasOne(br => br.Requestor)
                .WithMany()
                .HasForeignKey(br => br.RequestorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
