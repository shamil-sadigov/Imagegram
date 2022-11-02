using System.Data;
using Imagegram.Database;
using Microsoft.EntityFrameworkCore;

namespace Imagegram.Features;

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
    
}