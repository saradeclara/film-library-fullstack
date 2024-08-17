using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Film
{
    public class CreateFilmDto
    {
        public required string Title { get; set; }
    }
}