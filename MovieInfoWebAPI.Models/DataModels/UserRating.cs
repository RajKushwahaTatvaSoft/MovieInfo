using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MovieInfoWebAPI.Models.DataModels;

[Table("UserRating")]
public partial class UserRating
{
    [Key]
    public int RatingId { get; set; }

    public int UserId { get; set; }

    public int MovieId { get; set; }

    [Precision(4, 2)]
    public decimal Rating { get; set; }

    public string? Description { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime CreatedDate { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? ModifiedDate { get; set; }

    [ForeignKey("MovieId")]
    [InverseProperty("UserRatings")]
    public virtual MovieDetail Movie { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("UserRatings")]
    public virtual User User { get; set; } = null!;
}
