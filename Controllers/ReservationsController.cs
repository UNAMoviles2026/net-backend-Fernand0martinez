using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using reservations_api.DTOs.Requests;
using reservations_api.DTOs.Responses;
using reservations_api.Services;
using System.Globalization;

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

    [HttpGet]
    [ProducesResponseType(typeof(List<ReservationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllReservationsByDate([FromQuery, BindRequired] string date)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (string.IsNullOrWhiteSpace(date))
        {
            return BadRequest(new { message = "The 'date' query parameter is required and must be in yyyy-MM-dd format." });
        }

        var isValidDate = DateOnly.TryParseExact(
            date,
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var parsedDate);

        if (!isValidDate)
        {
            return BadRequest(new { message = "Invalid date format. Use yyyy-MM-dd." });
        }

        var reservations = await _reservationService.GetAllReservationsByDateAsync(parsedDate);
        return Ok(reservations ?? new List<ReservationResponse>());
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteReservationById([FromRoute] Guid id)
    {
        var reservationDeleted = await _reservationService.DeleteReservationByIdAsync(id);
        if (reservationDeleted == false)
        {
            return NotFound();
        }
        return NoContent();
    }

}
