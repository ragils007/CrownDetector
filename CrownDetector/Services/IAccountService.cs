using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrownDetector.Services
{
    public interface IAccountService
    {
        AppUser Login(string login, string password);
    }
}
