namespace CW_10_S29916.DTOs;

public class PagedTripsResponseDto
{
    public int PageNum { get; set; }
    public int PageSize { get; set; }
    public int AllPages { get; set; }

    public List<TripDto> Trips { get; set; } = [];
}