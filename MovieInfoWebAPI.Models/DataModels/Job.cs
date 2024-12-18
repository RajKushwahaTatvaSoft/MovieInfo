using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MovieInfoWebAPI.Models.DataModels;

[Table("Job")]
public partial class Job
{
    [Key]
    public int JobId { get; set; }

    [StringLength(255)]
    public string JobTitle { get; set; } = null!;

    public int DepartmentId { get; set; }

    [InverseProperty("Job")]
    public virtual ICollection<Crew> Crews { get; set; } = new List<Crew>();

    [ForeignKey("DepartmentId")]
    [InverseProperty("Jobs")]
    public virtual Department Department { get; set; } = null!;
}
