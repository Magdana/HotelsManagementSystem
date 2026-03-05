using HMS.Application.Interfaces.RepositoryInterfaces;
using HMS.Domain.Entities;
using HMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HMS.Infrastructure.Repositories;

public class RoomRepository : GenericRepository<Room, DbContext>, IRoomRepository
{
    public RoomRepository(AppDbContext context) : base(context)
    {
    }
}
