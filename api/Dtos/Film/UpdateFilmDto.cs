using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Film
{
    public class UpdateFilmDto
    {
        public const int MinLen = 5;
        public const int MaxLen = 100;


        [Required]
        [MinLength(MinLen, ErrorMessage = "Text must be at least {1} characters long.")]
        [MaxLength(MaxLen, ErrorMessage = "Text can only be {1} characters long.")]
        public string Title { get; set; } = string.Empty;
    }
}