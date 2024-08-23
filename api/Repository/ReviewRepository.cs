using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Review;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _context;
        public ReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        async public Task<Review?> CreateReviewAsync(int filmId, Review newReviewModel)
        {
            // check if film exists
            var foundFilm = await _context.Films.FindAsync(filmId);

            // if film is null, return null
            if (foundFilm == null)
            {
                return null;
            }

            await _context.Reviews.AddAsync(newReviewModel);
            await _context.SaveChangesAsync();

            return newReviewModel;
        }

        public Task<Review?> DeleteReviewAsync(int id)
        {
            throw new NotImplementedException();
        }

        async public Task<List<Review>> GetAllReviewsAsync()
        {
            return await _context.Reviews.ToListAsync();
        }

        async public Task<Review?> GetReviewByIdAsync(int id)
        {
            var foundReview = await _context.Reviews.FindAsync(id);

            if (foundReview == null)
            {
                return null;
            }

            return foundReview;
        }

        public Task<Review?> UpdateReviewAsync(int id, UpdateReviewDto updateReviewDto)
        {
            throw new NotImplementedException();
        }
    }
}