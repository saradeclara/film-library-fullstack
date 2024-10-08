using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Review;
using api.Interfaces;
using api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/reviews")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepo;
        private readonly IMapper _mapper;
        public ReviewController(IReviewRepository reviewRepo, IMapper mapper)
        {
            _reviewRepo = reviewRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var allReviews = await _reviewRepo.GetAllReviewsAsync();
            var mappedReviews = _mapper.Map<List<ReviewDto>>(allReviews);

            return Ok(mappedReviews);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var foundReview = await _reviewRepo.GetReviewByIdAsync(id);

            if (foundReview == null)
            {
                return NotFound();
            }
            var mappedFoundReview = _mapper.Map<ReviewDto>(foundReview);

            return Ok(mappedFoundReview);
        }

        [HttpPost]
        public async Task<IActionResult> Create(int filmId, [FromBody] CreateReviewDto createReviewDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var createdReviewModel = await _reviewRepo.CreateReviewAsync(filmId, createReviewDto);

            return CreatedAtAction(nameof(GetById), new { id = createdReviewModel?.Id }, _mapper.Map<ReviewDto>(createdReviewModel));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, UpdateReviewDto updateReviewDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }


            var updatedReviewModel = await _reviewRepo.UpdateReviewAsync(id, updateReviewDto);

            return Ok(_mapper.Map<ReviewDto>(updatedReviewModel));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var reviewToDelete = await _reviewRepo.DeleteReviewAsync(id);

            if (reviewToDelete == null)
            {
                return NotFound("No Review found");
            }

            return NoContent();
        }
    }
}