using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Review
{
    public class CreateReviewDto
    {
        public const int MinLen = 5;
        public const int MaxLen = 280;


        [Required]
        [MinLength(MinLen, ErrorMessage = "Text must be at least {1} characters long.")]
        [MaxLength(MaxLen, ErrorMessage = "Text can only be {1} characters long.")]
        public string Body { get; set; } = string.Empty;
    }
}