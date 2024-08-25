using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Film;
using api.Models;

namespace api.Interfaces
{
    public interface IFilmRepository
    {
        Task<List<Film>> GetAllFilmsAsync(string? filmTitle, string? sortBy, bool? isDescending);
        Task<Film?> GetFilmByIdAsync(int id);
        Task<Film> CreateFilmAsync(CreateFilmDto createFilmDto);
        Task<Film?> UpdateFilmAsync(int id, UpdateFilmDto updateFilmDto);
        Task<Film?> DeleteFilmAsync(int id);
    }
}