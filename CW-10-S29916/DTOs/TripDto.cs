using CW_10_S29916.Controllers;

namespace CW_10_S29916.DTOs;

public class TripDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int MaxPeople { get; set; }

    public List<CountryDto> Countries { get; set; } = [];
    public List<TripClientDto> Clients { get; set; } = [];
}