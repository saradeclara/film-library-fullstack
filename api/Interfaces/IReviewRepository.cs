using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Review;
using api.Models;

namespace api.Interfaces
{
    public interface IReviewRepository
    {
        Task<List<Review>> GetAllReviewsAsync();
        Task<Review?> GetReviewByIdAsync(int id);
        Task<Review?> CreateReviewAsync(int filmId, CreateReviewDto createReviewDto);
        Task<Review?> UpdateReviewAsync(int id, UpdateReviewDto updateReviewDto);
        Task<Review?> DeleteReviewAsync(int id);
    }
}