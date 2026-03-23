using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nails_trail.Models.Dtos
{
    public class DateAgendedResume
    {
		[Key]
		[Column("id_date")]
		public int IdDate { get; set; }

		[Column("idissuer")]
		[Required]
		[MaxLength(450)]
		public string IdIssuer { get; set; }

		[ForeignKey("IdIssuer")]
		public virtual AspNetUser User { get; set; }

		[Column("datereserved")]
		[Required]
		public DateTime DateReserved { get; set; }

		[Column("total_price")]
		public decimal? TotalPrice { get; set; }

		[Column("total_duration")]
		public int? TotalDuration { get; set; }

		[Column("status")]
		[MaxLength(20)]
		public string Status { get; set; } = "Pendiente";

		[Column("notes")]
		public string Notes { get; set; }

		[Column("created_at")]
		public DateTime CreatedAt { get; set; } = DateTime.Now;

		// Navegación a los detalles
		public virtual ICollection<DateServiceDetail> ServiceDetails { get; set; }
	}
}
