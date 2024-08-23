using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Review;

namespace api.Dtos.Film
{
    public class FilmDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public List<ReviewDto> Reviews { get; set; } = new List<ReviewDto>();
    }
}