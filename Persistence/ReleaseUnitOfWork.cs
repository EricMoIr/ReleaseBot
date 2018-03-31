using Persistence.Domain;
using System;
using System.Collections.Generic;

namespace Persistence
{
    public class ReleaseUnitOfWork 
    {
        private ReleaseContext context;
        public ReleaseRepository<Release> Releases { get; }
        public ReleaseRepository<Releasable> Releasables { get; }
        public ReleaseRepository<SourceSubscription> SourceSubscriptions { get; }
        public ReleaseRepository<ItemSubscription> ItemSubscriptions { get; }
        public ReleaseRepository<Subscriber> Subscribers { get; }
        public ReleaseRepository<Source> Sources { get; }

        private bool disposed = false;

        public ReleaseUnitOfWork()
        {
            context = new ReleaseContext();
            Releases = new ReleaseRepository<Release>(context);
            Releasables = new ReleaseRepository<Releasable>(context);
            ItemSubscriptions = new ReleaseRepository<ItemSubscription>(context);
            SourceSubscriptions = new ReleaseRepository<SourceSubscription>(context);
            Subscribers = new ReleaseRepository<Subscriber>(context);
            Sources = new ReleaseRepository<Source>(context);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
