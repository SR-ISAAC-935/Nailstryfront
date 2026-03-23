using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nails_trail.Models.Dtos
{
	[Table("Services")]
	public class Service
    {
		[Key]
		[Column("id_service")]
		public int IdService { get; set; }

		[Column("service_name")]
		[Required]
		[MaxLength(100)]
		public string ServiceName { get; set; }

		[Column("description")]
		[MaxLength(500)]
		public string Description { get; set; }

		[Column("price")]
		[Required]
		public decimal Price { get; set; }

		[Column("duration")]
		[Required]
		public int Duration { get; set; }

		[Column("is_active")]
		public bool IsActive { get; set; } = true;

		[Column("created_at")]
		public DateTime CreatedAt { get; set; } = DateTime.Now;

		// Navegación
		public virtual ICollection<DateServiceDetail> DateServiceDetails { get; set; }
	}
}
