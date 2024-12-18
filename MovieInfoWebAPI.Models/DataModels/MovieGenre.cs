using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MovieInfoWebAPI.Models.DataModels;

[Table("MovieGenre")]
public partial class MovieGenre
{
    [Key]
    public int GenreId { get; set; }

    public string GenreName { get; set; } = null!;

    [InverseProperty("Genre")]
    public virtual ICollection<MovieGenreMapping> MovieGenreMappings { get; set; } = new List<MovieGenreMapping>();
}
