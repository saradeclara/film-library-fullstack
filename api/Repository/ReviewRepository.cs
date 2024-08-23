using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Review;
using api.Interfaces;
using api.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public ReviewRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Review?> CreateReviewAsync(int filmId, CreateReviewDto createReviewDto)
        {
            // check if film exists
            var foundFilm = await _context.Films.FindAsync(filmId);
            // if film is null, return null
            if (foundFilm == null)
            {
                return null;
            }

            // map dto to model to add this to db
            var mappedCreateDto = _mapper.Map<Review>(createReviewDto);
            mappedCreateDto.FilmId = filmId;

            await _context.Reviews.AddAsync(mappedCreateDto);
            await _context.SaveChangesAsync();

            return mappedCreateDto;
        }

        public Task<Review?> DeleteReviewAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Review>> GetAllReviewsAsync()
        {
            return await _context.Reviews.ToListAsync();
        }

        public async Task<Review?> GetReviewByIdAsync(int id)
        {
            var foundReview = await _context.Reviews.FindAsync(id);

            if (foundReview == null)
            {
                return null;
            }

            return foundReview;
        }

        public async Task<Review?> UpdateReviewAsync(int id, UpdateReviewDto updateReviewDto)
        {
            var reviewToUpdate = await _context.Reviews.FirstOrDefaultAsync(x => x.Id == id);

            if (reviewToUpdate == null)
            {
                return null;
            }

            // update review
            _mapper.Map(updateReviewDto, reviewToUpdate);

            await _context.SaveChangesAsync();

            return reviewToUpdate;
        }
    }
}