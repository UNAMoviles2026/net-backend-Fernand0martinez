using reservations_api.DTOs.Requests;
using reservations_api.DTOs.Responses;

namespace reservations_api.Services;

public interface IReservationService
{
    Task<ReservationResponse> CreateAsync(CreateReservationRequest request);

    // This method is added to get all reservations for a specific date, regardless of the classroom
    Task<List<ReservationResponse>> GetAllReservationsByDateAsync(DateOnly date);

    Task<bool> DeleteReservationByIdAsync(Guid id);
}