using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Tournament.Core.Dto;
using Tournament.Core.Entities;

namespace Tournament.Data.Data;

public class TournamentMappings : Profile
{

    public TournamentMappings()
    {
        // Set up mapping between DTOs and entities for tournaments and games
        CreateMap<GameDto, Game>().ReverseMap();
        CreateMap<UpdateGameDto, Game>();
        CreateMap<CreateGameDto, Game>();

        CreateMap<TournamentDto, TournamentDetails>().ReverseMap();
        CreateMap<UpdateTournamentDto, TournamentDetails>();
        CreateMap<CreateTournamentDto, TournamentDetails>();
    }
}
