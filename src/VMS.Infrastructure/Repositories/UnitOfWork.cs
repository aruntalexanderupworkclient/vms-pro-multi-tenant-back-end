using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using VMS.Application.Interfaces;
using VMS.Core.Entities;
using VMS.Core.Interfaces;
using VMS.Infrastructure.Data;

namespace VMS.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly VmsDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ConcurrentDictionary<Type, object> _repositories = new();
    private bool _disposed;

    public UnitOfWork(VmsDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public IRepository<T> Repository<T>() where T : BaseEntity
    {
        return (IRepository<T>)_repositories.GetOrAdd(typeof(T), _ => new Repository<T>(_context));
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserService.UserId;

        foreach (var entry in _context.ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = currentUserId;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = currentUserId;

                    if (entry.Entity.IsDeleted && entry.Property(nameof(BaseEntity.IsDeleted)).IsModified)
                    {
                        entry.Entity.DeletedAt = DateTime.UtcNow;
                        entry.Entity.DeletedBy = currentUserId;
                    }
                    break;
            }
        }

        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _context.Dispose();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}
