using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MovieInfoWebAPI.Models.DataModels;

[Table("MovieGenreMapping")]
public partial class MovieGenreMapping
{
    [Key]
    public int SerialId { get; set; }

    public int MovieId { get; set; }

    public int GenreId { get; set; }

    [ForeignKey("GenreId")]
    [InverseProperty("MovieGenreMappings")]
    public virtual MovieGenre Genre { get; set; } = null!;

    [ForeignKey("MovieId")]
    [InverseProperty("MovieGenreMappings")]
    public virtual MovieDetail Movie { get; set; } = null!;
}
