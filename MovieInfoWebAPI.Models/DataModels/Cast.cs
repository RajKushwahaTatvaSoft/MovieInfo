using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MovieInfoWebAPI.Models.DataModels;

[Table("Cast")]
public partial class Cast
{
    [Key]
    public int CastId { get; set; }

    public int MovieId { get; set; }

    public int PersonId { get; set; }

    public string? Character { get; set; }

    public int? CastOrder { get; set; }

    [ForeignKey("MovieId")]
    [InverseProperty("Casts")]
    public virtual MovieDetail Movie { get; set; } = null!;

    [ForeignKey("PersonId")]
    [InverseProperty("Casts")]
    public virtual Person Person { get; set; } = null!;
}
