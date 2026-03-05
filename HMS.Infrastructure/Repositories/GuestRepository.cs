using HMS.Application.Interfaces.RepositoryInterfaces;
using HMS.Domain.Entities;
using HMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HMS.Infrastructure.Repositories;

public class GuestRepository : GenericRepository<Guest, DbContext>, IGuestRepository
{
    public GuestRepository(AppDbContext context) : base(context)
    {
    }

}
