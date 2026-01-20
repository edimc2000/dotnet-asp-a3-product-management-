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
    public string Name { get; set; } = null!;

    [Column("description", TypeName = "VARCHAR(100)")]
    public string Description { get; set; } = null!;

    [Column("price")]
    public double Price { get; set; }

    [Column("created_at", TypeName = "DATETIME")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "DATETIME")]
    public DateTime? UpdatedAt { get; set; }

    [Column("last_accessed_at", TypeName = "DATETIME")]
    public DateTime? LastAccessedAt { get; set; }

    [Column("last_accessed_by", TypeName = "VARCHAR(100)")]
    public string LastAccessedBy { get; set; } = null!;
}
