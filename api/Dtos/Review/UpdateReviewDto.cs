using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Review
{
    public class UpdateReviewDto
    {
        public int Id { get; set; }
        public required string Body { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}