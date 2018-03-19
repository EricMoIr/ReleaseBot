using System;
using System.Collections.Generic;
using Persistence.Domain;
using Persistence;

namespace Services
{
    internal class SourceService
    {
        private static ReleaseUnitOfWork uow = new ReleaseUnitOfWork();
        private static ReleaseRepository<Source> sources = uow.Sources;
        internal static IEnumerable<Source> GetAll()
        {
            return sources.Get();
        }
    }
}