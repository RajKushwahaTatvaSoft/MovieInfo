using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MovieInfoWebAPI.Models.DataModels;

[Table("MovieDetail")]
public partial class MovieDetail
{
    [Key]
    public int MovieId { get; set; }

    public string Title { get; set; } = null!;

    public DateOnly? ReleaseDate { get; set; }

    public long Revenue { get; set; }

    public int? Duration { get; set; }

    [Precision(10, 6)]
    public decimal Popularity { get; set; }

    public string PosterUrl { get; set; } = null!;

    public string ImdbId { get; set; } = null!;

    public long Budget { get; set; }

    [Precision(4, 2)]
    public decimal Rating { get; set; }

    public int RatingCount { get; set; }

    public string? Director { get; set; }

    public string? Writer { get; set; }

    public string? Actors { get; set; }

    public string? Awards { get; set; }

    public string? ShortPlot { get; set; }

    public string? LongPlot { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime CreatedDate { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? ModifiedDate { get; set; }

    public bool IsDeleted { get; set; }

    [InverseProperty("Movie")]
    public virtual ICollection<Cast> Casts { get; set; } = new List<Cast>();

    [InverseProperty("Movie")]
    public virtual ICollection<Crew> Crews { get; set; } = new List<Crew>();

    [InverseProperty("Movie")]
    public virtual ICollection<MovieGenreMapping> MovieGenreMappings { get; set; } = new List<MovieGenreMapping>();

    [InverseProperty("Movie")]
    public virtual ICollection<UserRating> UserRatings { get; set; } = new List<UserRating>();
}
