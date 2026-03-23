namespace nails_trail.Models.Dtos
{
    public class CreateCitaDto
    {
		public DateTime DateReserved { get; set; }
		public List<ServiceSelectionDto> Services { get; set; }
		public string Notes { get; set; }
	}
}
