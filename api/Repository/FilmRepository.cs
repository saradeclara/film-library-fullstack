using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Film;
using api.Interfaces;
using api.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class FilmRepository : IFilmRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public FilmRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Film> CreateFilmAsync(CreateFilmDto createFilmDto)
        {
            var createFilmModel = _mapper.Map<Film>(createFilmDto);

            await _context.Films.AddAsync(createFilmModel);
            await _context.SaveChangesAsync();

            return createFilmModel;
        }

        public async Task<Film?> DeleteFilmAsync(int id)
        {
            var filmToDelete = await _context.Films.FindAsync(id);


            if (filmToDelete == null)
            {
                return null;
            }

            _context.Films.Remove(filmToDelete);
            _context.SaveChanges();

            return filmToDelete;
        }

        public async Task<List<Film>> GetAllFilmsAsync(string? title, string? sortBy, bool? isDescending, int pageNumber, int pageSize)
        {
            var films = _context.Films.Include(el => el.Reviews).AsQueryable();

            if (!string.IsNullOrWhiteSpace(title))
            {
                films = films.Where(singleFilm => singleFilm.Title.Contains(title));
            }

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "title":
                        films = isDescending == true
                            ? films.OrderByDescending(film => film.Title)
                            : films.OrderBy(film => film.Title);
                        break;

                }
            }

            var skipNumber = (pageNumber - 1) * pageSize;
            var takeNumber = pageSize;

            return await films.Skip(skipNumber).Take(takeNumber).ToListAsync();
        }

        public async Task<Film?> GetFilmByIdAsync(int id)
        {
            return await _context.Films.Include(el => el.Reviews).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Film?> UpdateFilmAsync(int id, UpdateFilmDto updateFilmDto)
        {
            var filmToUpdate = await _context.Films.FirstOrDefaultAsync(x => x.Id == id);

            if (filmToUpdate == null)
            {
                return null;
            }

            // if film is found
            // update file
            _mapper.Map(updateFilmDto, filmToUpdate);

            // add to db
            // save changes
            await _context.SaveChangesAsync();

            return filmToUpdate;
        }
    }
}