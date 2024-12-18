using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MovieInfoWebAPI.Models.DataModels;

[Table("Person")]
public partial class Person
{
    [Key]
    public int PersonId { get; set; }

    [StringLength(255)]
    public string PersonName { get; set; } = null!;

    public int? Gender { get; set; }

    [StringLength(255)]
    public string? ProfilePath { get; set; }

    [InverseProperty("Person")]
    public virtual ICollection<Cast> Casts { get; set; } = new List<Cast>();

    [InverseProperty("Person")]
    public virtual ICollection<Crew> Crews { get; set; } = new List<Crew>();
}
