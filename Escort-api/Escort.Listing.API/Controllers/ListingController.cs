using Escort.Listing.API.DTO;
using Escort.Listing.Application.Repositories;
using Escort.Listing.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Escort.Listing.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ListingController : Controller
{
    private readonly IListingRepository _listingRepository;
    
    public ListingController(IListingRepository listingRepository)
    {
        _listingRepository = listingRepository;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllListings()
    {
        var listings = await _listingRepository.GetAllAsync();
        return Ok(listings.Select(listing => listing.ToDto()));
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetListingById(Guid id)
    {
        var listing = await _listingRepository.GetByIdAsync(id);
        return Ok(listing.ToDto());
    }
    
    [HttpPost]
    public async Task<ActionResult<IEnumerable<ListingGetDTO>>> CreateListing(ListingPostPutDto listingPostPutDto)
    {
        var listing = await _listingRepository.CreateAsync(listingPostPutDto.ToDomain());
        return CreatedAtAction(nameof(GetListingById), new { id = listing.Id }, listing.ToDto());
    }
    
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ListingGetDTO>> UpdateListing(Guid id, [FromBody] ListingPostPutDto listingPostPutDto)
    {
        {
            try
            {
                var listing = listingPostPutDto.ToDomain();
                listing.Id = id;
                await _listingRepository.UpdateAsync(listing);
                var listingGetDto = listing.ToDto();

                return Ok(listingGetDto);
            }
            catch (ModelNotFoundException)
            {
                return NotFound();
            }
        }
    }
}