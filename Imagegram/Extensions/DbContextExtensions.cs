using System.Data;
using Imagegram.Database;
using Imagegram.Database.Entities;
using Imagegram.Features;
using Microsoft.EntityFrameworkCore;

namespace Imagegram.Extensions;


// TODO: Add logging and extract to DbContext itself
public static class DbContextExtensions
{
    public static async Task InTransactionAsync(this ApplicationDbContext dbContext,
        IsolationLevel isolationLevel,
        Func<Task> dbOperation,
        CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
        
        try
        {
            await dbOperation();
        
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
    
    public static async Task<TBaseEntity> FindOrThrowAsync<TBaseEntity>(
        this IQueryable<TBaseEntity> dbSet,
        int postId,
        CancellationToken token)
        where TBaseEntity: BaseEntity
    {
        var entity = await dbSet.FirstOrDefaultAsync(x => x.Id == postId, token);
        
        if (entity is null)
        {
            throw new EntityNotFoundException($"{typeof(TBaseEntity).Name} was not found by Id '{postId}'");
        }

        return entity;
    }
    
}