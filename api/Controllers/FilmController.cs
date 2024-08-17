using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Film;
using api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/films")]
    [ApiController]
    public class FilmController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public FilmController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var films = await _context.Films.ToListAsync();
            var mappedFilms = _mapper.Map<List<FilmDto>>(films);
            return Ok(mappedFilms);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var singleFilm = await _context.Films.FindAsync(id);
            if (singleFilm == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<FilmDto>(singleFilm));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFilmDto createFilmDto)
        {
            var newFilmModel = _mapper.Map<Film>(createFilmDto);

            await _context.Films.AddAsync(newFilmModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newFilmModel.Id }, _mapper.Map<FilmDto>(newFilmModel));

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateFilmDto updateFilmDto)
        {
            // check body
            if (updateFilmDto == null)
            {
                return BadRequest("Invalid Data");
            }

            // find film record through id
            var foundFilm = await _context.Films.FirstOrDefaultAsync(film => film.Id == id);

            // if film is not found, return NotFound()
            if (foundFilm == null)
            {
                return NotFound("Film with " + id + " id was not found.");
            }

            // if film is found
            // update file
            _mapper.Map(updateFilmDto, foundFilm);

            // add to db
            // save changes
            await _context.SaveChangesAsync();

            // return updated record
            return Ok(_mapper.Map<FilmDto>(foundFilm));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            // find film
            var filmToDelete = await _context.Films.FirstOrDefaultAsync(film => film.Id == id);

            // if not found, return NotFound()
            if (filmToDelete == null)
            {
                return NotFound("Film with " + id + " id was not found.");
            }

            // if found, delete from db
            _context.Films.Remove(filmToDelete);
            // save changes
            _context.SaveChanges();

            // return nocontent
            return NoContent();
        }
    }
}