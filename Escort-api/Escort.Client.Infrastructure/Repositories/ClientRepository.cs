using Escort.Client.Application.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Escort.Client.Infrastructure.Repositories;

public class ClientRepository: BaseRepository<Domain.Models.Client>, IClientRepository
{
    public ClientRepository(DbContext context) : base(context)
    {
    }
}