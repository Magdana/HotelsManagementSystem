using HMS.Application.Interfaces.RepositoryInterfaces;
using HMS.Domain.Entities;
using HMS.Infrastructure.Data;

namespace HMS.Infrastructure.Repositories;

public class ManagerRepository : GenericRepository<Manager, AppDbContext>, IManagerRepository
{
    public ManagerRepository(AppDbContext context) : base(context)
    {
    }
}
