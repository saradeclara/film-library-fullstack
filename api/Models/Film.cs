using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class Film
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public List<Actor> Cast { get; set; } = new List<Actor>();
        public List<Director> DirectedBy { get; set; } = new List<Director>();
    }
}