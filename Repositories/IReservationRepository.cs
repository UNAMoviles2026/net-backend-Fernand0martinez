using reservations_api.Models.Entities;

namespace reservations_api.Repositories;

public interface IReservationRepository
{
  Task<Reservation> AddAsync(Reservation reservation);
  Task<List<Reservation>> GetByClassroomAndDateAsync(Guid classroomId, DateOnly date);

  // This method is added to get all reservations for a specific date, regardless of the classroom
  Task<List<Reservation>> GetALLReservationByDateAsync(DateOnly date);
}