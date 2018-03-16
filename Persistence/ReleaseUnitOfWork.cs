﻿using Persistence.Domain;
using System;
using System.Collections.Generic;

namespace Persistence
{
    public class ReleaseUnitOfWork 
    {
        private ReleaseContext context;
        public ReleaseRepository<ReleaseDate> ReleaseDates { get; }
        public ReleaseRepository<Releasable> Releasables { get; }
        public ReleaseRepository<SourceSubscription> SourceSubscriptions { get; }
        public ReleaseRepository<ItemSubscription> ItemSubscriptions { get; }
        public ReleaseRepository<User> Users { get; }
        public ReleaseRepository<Source> Sources { get; }
        public ReleaseRepository<DOM> DOMs { get; }

        private bool disposed = false;

        public ReleaseUnitOfWork()
        {
            context = new ReleaseContext();
            ReleaseDates = new ReleaseRepository<ReleaseDate>(context);
            Releasables = new ReleaseRepository<Releasable>(context);
            ItemSubscriptions = new ReleaseRepository<ItemSubscription>(context);
            SourceSubscriptions = new ReleaseRepository<SourceSubscription>(context);
            Users = new ReleaseRepository<User>(context);
            Sources = new ReleaseRepository<Source>(context);
            DOMs = new ReleaseRepository<DOM>(context);
        }

        public ReleaseUnitOfWork(ReleaseContext releaseContext)
        {
            context = releaseContext;
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
