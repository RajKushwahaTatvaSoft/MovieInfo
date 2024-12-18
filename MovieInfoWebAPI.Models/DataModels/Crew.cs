using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MovieInfoWebAPI.Models.DataModels;

[Table("Crew")]
public partial class Crew
{
    [Key]
    public int CrewId { get; set; }

    public int MovieId { get; set; }

    public int PersonId { get; set; }

    public int JobId { get; set; }

    [ForeignKey("JobId")]
    [InverseProperty("Crews")]
    public virtual Job Job { get; set; } = null!;

    [ForeignKey("MovieId")]
    [InverseProperty("Crews")]
    public virtual MovieDetail Movie { get; set; } = null!;

    [ForeignKey("PersonId")]
    [InverseProperty("Crews")]
    public virtual Person Person { get; set; } = null!;
}
