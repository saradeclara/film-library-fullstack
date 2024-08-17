using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Film;
using api.Models;
using AutoMapper;

namespace api.Mappers
{
    public class FilmMappingProfile : Profile
    {
        public FilmMappingProfile()
        {
            CreateMap<Film, FilmDto>();
            CreateMap<CreateFilmDto, Film>();
            CreateMap<UpdateFilmDto, Film>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }

    }
}