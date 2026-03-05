using HMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HMS.Application.Interfaces.RepositoryInterfaces;

public interface IRoomRepository:IGenericRepository<Room, DbContext>
{
}
