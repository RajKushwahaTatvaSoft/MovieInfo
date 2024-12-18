using AutoMapper;
using MovieInfoWebAPI.Models.CustomModels;
using MovieInfoWebAPI.Models.DataModels;
using MovieInfoWebAPI.Models.DTO.CastDTO;
using MovieInfoWebAPI.Models.DTO.MovieDTO;
using MovieInfoWebAPI.Models.DTO.UserDTO;
using MovieInfoWebAPI.Models.DTO.UserRatingDTO;
using MovieInfoWebAPI.Services.Utilities;

namespace MovieInfoWebAPI
{
    public class MappingConfig : Profile
    {
        private readonly string dateFormat = "f";
        public MappingConfig()
        {
            CreateMap<UserCreateDTO, User>().ReverseMap();
            CreateMap<UserUpdateDTO, User>().ForMember(user => user.Email, m => m.MapFrom(udto => udto.UserEmail));
            CreateMap<User, UserResponseDTO>().ForMember(udto => udto.UserRole, m => m.MapFrom(user => user.Role.RoleName)).ForMember(udto => udto.ProfilePath, m => m.MapFrom(user => user.ProfilePhotoName != null ? $"/ServerData/User/{user.UserId}/{user.ProfilePhotoName}" : null));

            CreateMap<UserRating, RatingCreateDTO>().ReverseMap();

            CreateMap(typeof(PaginatedResponse<>), typeof(PaginatedResponse<>)).ConvertUsing(typeof(PagedListConverter<,>));

            CreateMap<MovieDetail, MovieResponseDTO>()
                .ForMember(mres => mres.ReleaseDateStr,
                m => m.MapFrom(mdet => mdet.ReleaseDate == null
                ? null
                : mdet.ReleaseDate.Value.ToString("dd-MM-yyyy"))).ForMember(mres => mres.Overview, m => m.MapFrom(mdet => mdet.ShortPlot));

            CreateMap<UserRating, RatingResponseDTO>()
                .ForMember(rr => rr.UserName, m => m.MapFrom(ur => $"{ur.User.FirstName} {ur.User.LastName}"))
                .ForMember(rr => rr.ReviewDate, m => m.MapFrom(ur => ur.ModifiedDate == null ? ur.CreatedDate.ToString(dateFormat) : ur.ModifiedDate.Value.ToString(dateFormat)))
                .ForMember(rr => rr.IsModified, m => m.MapFrom(ur => ur.ModifiedDate != null))
                .ForMember(rr => rr.UserProfileUrl, m => m.MapFrom(ur => ur.User.ProfilePhotoName == null ? ImageHelper.GetUIAvatarURL(ur.User.FirstName, ur.User.LastName) : ImageHelper.GetUserProfileURL(ur.User.ProfilePhotoName, ur.User.UserId)));

            CreateMap<MovieCreateDTO, MovieDetail>()
                .ForMember(md => md.ShortPlot, m => m.MapFrom(mc => mc.OverView))
                .ForMember(md => md.LongPlot, m => m.MapFrom(mc => mc.Plot))
                .ForMember(md => md.Actors, m => m.MapFrom(mc => mc.Actor));

            CreateMap<Cast, CastDetailResponseDTO>()
                .ForMember(dest => dest.MovieId, m => m.MapFrom(src => src.Movie.MovieId))
                .ForMember(dest => dest.Title, m => m.MapFrom(src => src.Movie.Title))
                .ForMember(dest => dest.PosterUrl, m => m.MapFrom(src => src.Movie.PosterUrl));

            CreateMap<MovieDetail, MovieTileResponseDTO>();
        }
    }
}

public class PagedListConverter<TSource, TDestination> : ITypeConverter<PaginatedResponse<TSource>, PaginatedResponse<TDestination>> where TDestination : class where TSource : class
{
    public PaginatedResponse<TDestination> Convert(PaginatedResponse<TSource> source, PaginatedResponse<TDestination> destination, ResolutionContext context)
    {
        var mappedItems = context.Mapper.Map<List<TDestination>>(source.Data);
        return new PaginatedResponse<TDestination>(mappedItems, source.TotalCount, source.CurrentPage, source.PageSize);
    }
}

