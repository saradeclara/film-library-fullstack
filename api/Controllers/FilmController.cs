using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Film;
using api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult GetAll()
        {
            var films = _context.Films.ToList();
            var mappedFilms = _mapper.Map<List<FilmDto>>(films);
            return Ok(mappedFilms);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var singleFilm = _context.Films.Find(id);
            if (singleFilm == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<FilmDto>(singleFilm));
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateFilmDto createFilmDto)
        {
            var newFilmModel = _mapper.Map<Film>(createFilmDto);

            _context.Films.Add(newFilmModel);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = newFilmModel.Id }, _mapper.Map<FilmDto>(newFilmModel));

        }
    }
}