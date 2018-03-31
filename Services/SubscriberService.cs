using System;
using Persistence.Domain;
using Persistence;
using System.Linq;

namespace Services
{
    internal class SubscriberService
    {
        private static ReleaseUnitOfWork uow = new ReleaseUnitOfWork();
        private static ReleaseRepository<Subscriber> subscribers = uow.Subscribers;
        internal static Subscriber GetOrCreate(string name)
        {
            Subscriber subscriber = subscribers.Get(x => x.Name == name).FirstOrDefault();
            if(subscriber == null)
            {
                subscriber = new Subscriber()
                {
                    Name = name
                };
                subscribers.Insert(subscriber);
                uow.Save();
            }
            return subscriber;
        }
    }
}