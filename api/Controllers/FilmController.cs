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
        public async Task<IActionResult> GetAll()
        {
            var films = await _filmRepo.GetAllFilmsAsync();
            var mappedFilms = _mapper.Map<List<FilmDto>>(films);

            return Ok(mappedFilms);
        }

        [HttpGet("{id:int}")]
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
        public async Task<IActionResult> Create([FromBody] CreateFilmDto createFilmDto)
        {

            var createdFilmModel = await _filmRepo.CreateFilmAsync(createFilmDto);

            return CreatedAtAction(nameof(GetById), new { id = createdFilmModel?.Id }, _mapper.Map<FilmDto>(createdFilmModel));

        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateFilmDto updateFilmDto)
        {
            // check body
            if (updateFilmDto == null)
            {
                return BadRequest("Invalid Data");
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
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await _filmRepo.DeleteFilmAsync(id);

            // return nocontent
            return NoContent();
        }
    }
}