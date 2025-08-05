using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppService.Core.Entities;

[Table("PCTemporalAjustes")]
public class PCTipoPagoOrdenNoCalcularComision
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int IdTipoPago { get; set; }
    [Required]
    [StringLength(50)]
    public string DescripcionTipoPago { get; set; } = string.Empty;
    [Required]
    public long Orden { get; set; }

    [Required]
    [StringLength(600)]
    public string Cliente { get; set; }
    
    [Required]
    [StringLength(900)]
    public string SearchText { get; set; }
    

    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    [Required]
    [StringLength(50)]
    public string UsuarioCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    [StringLength(50)]
    public string? UsuarioActualizacion { get; set; }
}