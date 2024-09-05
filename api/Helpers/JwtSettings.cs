using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Helpers
{
    public class JwtSettings
    {
        public required string Key { get; set; }
        public int ExpirationInMinutes { get; set; }
    }
}