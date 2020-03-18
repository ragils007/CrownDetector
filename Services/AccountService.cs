using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrownDetector.Services
{
    public class AccountService : IAccountService
    {
        public AppUser Login(string login, string password)
        {
            return new AppUser() { Username = "test" };
        }
    }
}
