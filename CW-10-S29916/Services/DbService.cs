using CW_10_S29916.Controllers;
using CW_10_S29916.DTOs;
using CW_10_S29916.Models;
using Microsoft.EntityFrameworkCore;

namespace CW_10_S29916.Services;

public interface IDbService
{
    Task<PagedTripsResponseDto> GetPagedTripsAsync(int pageNum, int pageSize);
    Task<bool> ClientHasAnyTripsAsync(int clientId);
    Task<bool> DeleteClientAsync(int clientId);
    Task<bool> TripExistsAsync(int tripId);
    Task<bool> TripHasStartedAsync(int tripId);
    Task<bool> ClientByPeselExistsAsync(string pesel);
    Task<bool> ClientAlreadyOnTripAsync(string pesel, int tripId);
    Task AddClientToTripAsync(int tripId, CreateClientTripDto dto);
}

public class DbService(TravelDbContext context) : IDbService
{
    public async Task<PagedTripsResponseDto> GetPagedTripsAsync(int pageNum, int pageSize)
    {
        if (pageNum < 1) pageNum = 1;
        if (pageSize < 1) pageSize = 10;

        var totalTrips = await context.Trips.CountAsync();
        var allPages = (int)Math.Ceiling(totalTrips / (double)pageSize);

        var tripsQuery = context.Trips
            .OrderByDescending(t => t.DateFrom)
            .Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .Include(t => t.IdCountries)
            .Include(t => t.ClientTrips)
            .ThenInclude(ct => ct.IdClientNavigation);

        var tripsList = await tripsQuery.ToListAsync();

        var result = new PagedTripsResponseDto
        {
            PageNum = pageNum,
            PageSize = pageSize,
            AllPages = allPages
        };

        foreach (var trip in tripsList)
        {
            var dto = new TripDto
            {
                Name = trip.Name,
                Description = trip.Description,
                DateFrom = trip.DateFrom,
                DateTo = trip.DateTo,
                MaxPeople = trip.MaxPeople
            };


            dto.Countries = trip.IdCountries
                .Select(c => new CountryDto { Name = c.Name })
                .ToList();


            dto.Clients = trip.ClientTrips
                .Select(ct => new TripClientDto
                {
                    FirstName = ct.IdClientNavigation.FirstName,
                    LastName = ct.IdClientNavigation.LastName
                })
                .ToList();

            result.Trips.Add(dto);
        }

        return result;
    }

    public async Task<bool> ClientHasAnyTripsAsync(int clientId)
    {
        return await context.Client_Trips.AnyAsync(ct => ct.IdClient == clientId);
    }

    public async Task<bool> DeleteClientAsync(int clientId)
    {
        var client = await context.Clients.FindAsync(clientId);
        if (client == null) return false;

        context.Clients.Remove(client);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> TripExistsAsync(int tripId)
    {
        return await context.Trips.AnyAsync(t => t.IdTrip == tripId);
    }

    public async Task<bool> TripHasStartedAsync(int tripId)
    {
        var trip = await context.Trips.FindAsync(tripId);
        if (trip == null) return false;
        return trip.DateFrom <= DateTime.Now;
    }

    public async Task<bool> ClientByPeselExistsAsync(string pesel)
    {
        return await context.Clients.AnyAsync(c => c.Pesel == pesel);
    }

    public async Task<bool> ClientAlreadyOnTripAsync(string pesel, int tripId)
    {
        var client = await context.Clients.FirstOrDefaultAsync(c => c.Pesel == pesel);
        if (client == null) return false;
        return await context.Client_Trips.AnyAsync(ct => ct.IdClient == client.IdClient && ct.IdTrip == tripId);
    }

    public async Task AddClientToTripAsync(int tripId, CreateClientTripDto dto)
    {
        var newClient = new Client
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Telephone = dto.Telephone,
            Pesel = dto.Pesel
        };

        await context.Clients.AddAsync(newClient);
        await context.SaveChangesAsync();


        var clientTrip = new ClientTrip
        {
            IdClient = newClient.IdClient,
            IdTrip = tripId,
            RegisteredAt = DateTime.Now,
            PaymentDate = dto.PaymentDate
        };

        await context.Client_Trips.AddAsync(clientTrip);
        await context.SaveChangesAsync();
    }
}