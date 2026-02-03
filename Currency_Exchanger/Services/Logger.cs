using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Services
{
    class Logger
    {
        static readonly object _lock = new();

        public static void Log(string message)
        {
            lock (_lock)
            {
                File.AppendAllText(
                    "server.log",
                    $"[{DateTime.Now:yyyy-MM-dd || HH:mm:ss}] {message}{Environment.NewLine}"
                    );
            }
        }
    }
}
