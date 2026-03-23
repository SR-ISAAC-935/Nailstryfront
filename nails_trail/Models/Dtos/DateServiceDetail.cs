using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nails_trail.Models.Dtos
{
	[Table("DateServiceDetails")]
	public class DateServiceDetail
    {
		[Key]
		[Column("id_detail")]
		public int IdDetail { get; set; }

		[Column("id_date")]
		[Required]
		public int IdDate { get; set; }

		[Column("id_service")]
		[Required]
		public int IdService { get; set; }

		[Column("service_price")]
		[Required]
		public decimal ServicePrice { get; set; }

		[Column("service_duration")]
		[Required]
		public int ServiceDuration { get; set; }

		[Column("quantity")]
		public int Quantity { get; set; } = 1;

		[Column("notes")]
		[MaxLength(500)]
		public string Notes { get; set; }

		// Navegación
		[ForeignKey("IdDate")]
		public virtual DateAgendedResume DateAgended { get; set; }

		[ForeignKey("IdService")]
		public virtual Service Service { get; set; }
	}
}
