using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class BlacklistedToken
    {
        public int Id { get; set; }
        public string Jti { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
    }
}