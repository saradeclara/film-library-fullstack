using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Film
{
    public class FilmDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
    }
}