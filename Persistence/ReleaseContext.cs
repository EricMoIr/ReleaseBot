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
        public DbSet<ReleaseDate> ReleaseDates { get; }
        public DbSet<Releasable> Releasables { get; }
        public DbSet<SourceSubscription> SourceSubscriptions { get; }
        public DbSet<ItemSubscription> ItemSubscriptions { get; }
        public DbSet<User> Users { get; }
        public DbSet<Source> Sources { get; }
        public DbSet<DOM> DOMs { get; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
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