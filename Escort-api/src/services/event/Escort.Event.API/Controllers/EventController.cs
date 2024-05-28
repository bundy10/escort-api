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
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetEventById(int id)
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
    
    [HttpPut("{id:int}")]
    public async Task<ActionResult<EventGetDTO>> UpdateEvent(int id, [FromBody] EventPostPutDto eventPostPutDto)
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
    
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<EventGetDTO>> DeleteEvent(int id)
    {
        {
            try
            {
                var @event = await _eventRepository.DeleteAsync(id);
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