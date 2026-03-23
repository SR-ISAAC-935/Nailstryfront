namespace nails_trail.Models.Dtos
{
    public class CitaDetalleDto
    {
		public int IdDate { get; set; }
		public DateTime DateReserved { get; set; }
		public string Status { get; set; }
		public decimal TotalPrice { get; set; }
		public int TotalDuration { get; set; }
		public string ClientName { get; set; }
		public string ClientEmail { get; set; }
		public string ClientPhone { get; set; }
		public List<ServiceDetailDto> Services { get; set; }
	}
}
