using HMS.Application.Interfaces.RepositoryInterfaces;
using HMS.Domain.Entities;
using HMS.Infrastructure.Data;

namespace HMS.Infrastructure.Repositories;

public class HotelRepository: GenericRepository<Hotel, AppDbContext>, IHotelRepository
{
    public HotelRepository(AppDbContext context) : base(context)
    {
    }
}
