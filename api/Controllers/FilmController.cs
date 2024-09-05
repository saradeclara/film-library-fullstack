using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Film;
using api.Interfaces;
using api.Models;
using api.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/films")]
    [ApiController]
    public class FilmController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IFilmRepository _filmRepo;
        public FilmController(IMapper mapper, IFilmRepository filmRepo)
        {
            _mapper = mapper;
            _filmRepo = filmRepo;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] string? title, [FromQuery] string? sortBy, [FromQuery] bool? isDescending, [FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            var films = await _filmRepo.GetAllFilmsAsync(title, sortBy, isDescending, pageNumber, pageSize);
            var mappedFilms = _mapper.Map<List<FilmDto>>(films);

            return Ok(mappedFilms);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var singleFilm = await _filmRepo.GetFilmByIdAsync(id);

            if (singleFilm == null)
            {
                return NotFound("Film not found");
            }

            return Ok(_mapper.Map<FilmDto>(singleFilm));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateFilmDto createFilmDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var createdFilmModel = await _filmRepo.CreateFilmAsync(createFilmDto);

            return CreatedAtAction(nameof(GetById), new { id = createdFilmModel?.Id }, _mapper.Map<FilmDto>(createdFilmModel));

        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateFilmDto updateFilmDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var foundFilm = await _filmRepo.UpdateFilmAsync(id, updateFilmDto);

            if (foundFilm == null)
            {
                return NotFound("Film not found");
            }

            // return updated record
            return Ok(_mapper.Map<FilmDto>(foundFilm));
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await _filmRepo.DeleteFilmAsync(id);

            return NoContent();
        }
    }
}