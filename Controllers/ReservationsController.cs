using Microsoft.AspNetCore.Mvc;
using reservations_api.DTOs.Requests;
using reservations_api.DTOs.Responses;
using reservations_api.Services;

namespace reservations_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
  private readonly IReservationService _reservationService;

  public ReservationsController(IReservationService reservationService)
  {
    _reservationService = reservationService;
  }

  [HttpPost]
  public async Task<IActionResult> Create([FromBody] CreateReservationRequest request)
  {
    if (!ModelState.IsValid)
    {
      return ValidationProblem(ModelState);
    }

    try
    {
      var createdReservation = await _reservationService.CreateAsync(request);
      return CreatedAtAction(
          nameof(Create),
          createdReservation);
    }
    catch (InvalidOperationException ex)
    {
      if (ex.Message.Contains("StartTime"))
      {
        return BadRequest(new { message = ex.Message });
      }

      if (ex.Message.Contains("Time conflict"))
      {
        return Conflict(new { message = ex.Message });
      }

      throw;
    }
  }

  // This endpoint is added to get all reservations for a specific date, regardless of the classroom
  [HttpGet]
  public async Task<IActionResult> GetAllReservationsByDate([FromQuery] DateOnly date)
  {
    var reservations = await _reservationService.GetAllReservationsByDateAsync(date);
  // If there are no reservations for the given date, return an empty list instead of null
    return Ok(reservations ?? new List<ReservationResponse>());
  }
}