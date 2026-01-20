using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProductManagement;

[Table("products")]
public partial class Product
{
    [Key]
    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("name", TypeName = "VARCHAR(100)")]
    [Required]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be at least 2 characters long")]
    public string Name { get; set; } = null!;

    [Column("description", TypeName = "VARCHAR(100)")]
    [Required]
    [StringLength(100,
        MinimumLength = 2,
        ErrorMessage = "Description must be at least 2 characters long")]
    public string Description { get; set; } = null!;

    [Column("price")]
    [Required]
    [Range(0, 999999.0, ErrorMessage = "Price must be between 0 and 999,999")]
    public double Price { get; set; }


    [Column("created_at", TypeName = "DATETIME")]
    public DateTime? CreatedAt { get; set; }

    [Column("created_by", TypeName = "VARCHAR (100)")]
    public string? CreatedBy { get; set; }

    [Column("updated_at", TypeName = "DATETIME")]
    public DateTime? UpdatedAt { get; set; }

    [Column("updated_by", TypeName = "VARCHAR (100)")]
    public string? UpdatedBy { get; set; }

    [Column("last_accessed_at", TypeName = "DATETIME")]
    public DateTime? LastAccessedAt { get; set; }

    [Column("last_accessed_by", TypeName = "VARCHAR (100)")]
    public string? LastAccessedBy { get; set; }
}
