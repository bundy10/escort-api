using Escort.Event.API.DTO;
using Escort.Event.Application.Repositories;
using Escort.Event.Application.Services;
using Escort.Event.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Escort.Event.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventController : Controller
{
    private readonly IEventService _eventService;
    
    public EventController(IEventService eventService)
    {
        _eventService = eventService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllEvents()
    {
        var events = await _eventService.GetAllAsync();
        return Ok(events.Select(@event => @event.ToDto()));
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetEventById(int id)
    {
        var @event = await _eventService.GetByIdAsync(id);
        return Ok(@event.ToDto());
    }
    
    [HttpPost]
    public async Task<ActionResult<IEnumerable<EventGetDTO>>> CreateEvent(EventPostPutDto eventPostPutDto)
    {
        var @event = await _eventService.CreateAsync(eventPostPutDto.ToDomain());
        return CreatedAtAction(nameof(GetEventById), new { id = @event.Id }, @event.ToDto());
    }
    
    [HttpPut("{id:int}")]
    public async Task<ActionResult<EventGetDTO>> UpdateEvent(int id, [FromBody] EventPostPutDto eventPostPutDto)
    {
        {
            try
            {
                var @event = eventPostPutDto.ToDomain();
                @event.Id = id;
                await _eventService.UpdateAsync(@event);
                var eventGetDto = @event.ToDto();

                return Ok(eventGetDto);
            }
            catch (ModelNotFoundException)
            {
                return NotFound();
            }
        }
    }
    
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<EventGetDTO>> DeleteEvent(int id)
    {
        {
            try
            {
                var @event = await _eventService.DeleteAsync(id);
                var eventGetDto = @event.ToDto();

                return Ok(eventGetDto);
            }
            catch (ModelNotFoundException)
            {
                return NotFound();
            }
        }
    }
    [HttpGet("User/{id:int}")]
    public async Task<IActionResult> GetEventByUserId(int id)
    {
        var events = await _eventService.GetByUserIdAsync(id);
        return Ok(events.Select(@event => @event.ToDto()));
    }
}