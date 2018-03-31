using Persistence.Domain;
using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace Persistence
{
    public class ReleaseContext : DbContext
    {
        public ReleaseContext() : base("name=ReleaseContext") { }
        public DbSet<Release> ReleaseDates { get; set; }
        public DbSet<Releasable> Releasables { get; set; }
        public DbSet<SourceSubscription> SourceSubscriptions { get; set; }
        public DbSet<ItemSubscription> ItemSubscriptions { get; set; }
        public DbSet<Subscriber> Users { get; set; }
        public DbSet<Source> Sources { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            modelBuilder.Entity<Release>()
                .HasKey(r => new { r.Chapter, r.ReleasableTitle, r.SourceURL, r.TimePublished});
            modelBuilder.Entity<ItemSubscription>()
                .HasKey(i => new { i.ReleasableTitle, i.SubscribeeName });
            modelBuilder.Entity<SourceSubscription>()
                .HasKey(i => new { i.SourceURL, i.SubscribeeName });
        }

        public virtual EntityState GetState(object entity)
        {
            return Entry(entity).State;
        }

        public virtual void SetModified(object entity)
        {
            Entry(entity).State = EntityState.Modified;
        }
    }
}