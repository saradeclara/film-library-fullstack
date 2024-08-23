using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Review
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public required string Body { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public int? FilmId { get; set; }
    }
}