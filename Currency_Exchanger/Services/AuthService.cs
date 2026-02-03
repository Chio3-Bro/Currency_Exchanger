using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Services
{
    class AuthService
    {
        private Dictionary<string, string> users = new()
        {
            { "user", "user" },
            {"vetal", "vetal" },
            {"admin", "admin" }
        };

        public bool Login (string username, string password)
        {
            if (!users.ContainsKey(username))
                return false;

            return users[username] == password;
        }
    }
}
