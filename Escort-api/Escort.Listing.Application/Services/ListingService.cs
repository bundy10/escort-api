using System.ComponentModel.Design;
using Escort.Listing.Application.Repositories;
using Escort.Listing.Domain.Models;

namespace Escort.Listing.Application.Services;

public class ListingService : IListingService
{
    private readonly IListingRepository _listingRepository;
    
    public ListingService(IListingRepository listingRepository)
    {
        _listingRepository = listingRepository;
    }
    
    public async Task<Domain.Models.Listing> CreateAsync(ListingDetails listingDetails, int userId)
    {
        var listing = new Domain.Models.Listing(listingDetails, userId);
        return await _listingRepository.CreateAsync(listing);
    }
    
    public async Task<IEnumerable<Domain.Models.Listing>> GetAllAsync()
    {
        return await _listingRepository.GetAllAsync();
    }
    
    public async Task<Domain.Models.Listing> GetByIdAsync(int id)
    {
        return await _listingRepository.GetByIdAsync(id);
    }
    
    public async Task<Domain.Models.Listing> UpdateAsync(ListingDetails listingDetails, int userId)
    {
        var listing = new Domain.Models.Listing(listingDetails, userId);
        return await _listingRepository.UpdateAsync(listing);
    }
    
    public async Task<Domain.Models.Listing> DeleteAsync(int id)
    {
        return await _listingRepository.DeleteAsync(id);
    }
}