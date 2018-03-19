using System;
using Persistence.Domain;
using Persistence;
using System.Linq;

namespace Services
{
    internal class UserService
    {
        private static ReleaseUnitOfWork uow = new ReleaseUnitOfWork();
        private static ReleaseRepository<User> users = uow.Users;
        internal static User GetOrCreate(string name)
        {
            User user = users.Get(x => x.Name == name).FirstOrDefault();
            if(user == null)
            {
                user = new User()
                {
                    Name = name
                };
                users.Insert(user);
                uow.Save();
            }
            return user;
        }
    }
}