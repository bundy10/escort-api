using Escort.Event.API.DTO;
using Escort.Event.Application.Repositories;
using Escort.Event.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Escort.Event.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventController : Controller
{
    private readonly IEventRepository _eventRepository;
    
    public EventController(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllEvents()
    {
        var events = await _eventRepository.GetAllAsync();
        return Ok(events.Select(@event => @event.ToDto()));
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetEventById(Guid id)
    {
        var @event = await _eventRepository.GetByIdAsync(id);
        return Ok(@event.ToDto());
    }
    
    [HttpPost]
    public async Task<ActionResult<IEnumerable<EventGetDTO>>> CreateEvent(EventPostPutDto eventPostPutDto)
    {
        var @event = await _eventRepository.CreateAsync(eventPostPutDto.ToDomain());
        return CreatedAtAction(nameof(GetEventById), new { id = @event.Id }, @event.ToDto());
    }
    
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<EventGetDTO>> UpdateEvent(Guid id, [FromBody] EventPostPutDto eventPostPutDto)
    {
        {
            try
            {
                var @event = eventPostPutDto.ToDomain();
                @event.Id = id;
                await _eventRepository.UpdateAsync(@event);
                var eventGetDto = @event.ToDto();

                return Ok(eventGetDto);
            }
            catch (ModelNotFoundException)
            {
                return NotFound();
            }
        }
    }
    
}