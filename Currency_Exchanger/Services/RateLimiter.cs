using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Services
{
    class RateLimiter
    {
        private int requestCount = 0;
        private DateTime startTime = DateTime.Now;
        private const int LIMIT = 10;

        public bool Allow()
        {
            if ((DateTime.Now - startTime).TotalMinutes >= 1)
            {
                startTime = DateTime.Now;
                requestCount = 0;
            }

            requestCount++;

            return requestCount <= LIMIT;
        }
    }
}
