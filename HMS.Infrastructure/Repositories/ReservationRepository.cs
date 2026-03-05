using HMS.Application.Interfaces.RepositoryInterfaces;
using HMS.Domain.Entities;
using HMS.Infrastructure.Data;

namespace HMS.Infrastructure.Repositories;

public class ReservationRepository : GenericRepository<Reservation, AppDbContext>, IReservationRepository
{
    public ReservationRepository(AppDbContext context) : base(context)
    { }
}
