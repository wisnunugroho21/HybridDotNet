using General.Common.Query;
using General.Common.Result;
using General.Data;
using General.Data.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace General.Common.Services;

public class CrudService<T>(AppDbContext db, ILogger<CrudService<T>> logger)
    where T : Entity
{
    public virtual async Task<ListDataResult<T>> GetAll(int skip, int take, List<Filter> filters,
        List<Sort> sorts, CancellationToken cancellationToken)
    {
        return await db.Set<T>().AsNoTracking()
            .ToListDataResultAsync(skip, take, filters, sorts, cancellationToken);
    }

    public virtual async Task<DataResult<T?>> Get(long id, CancellationToken cancellationToken)
    {
        try
        {
            var data = await db.Set<T>().AsNoTracking().FirstAsync(x => x.Id == id, cancellationToken);
            return new DataResult<T?>(data);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message, e);
            
            return new DataResult<T?>(null)
            {
                Errors = [e.Message]
            };
        }
    }

    public virtual async Task<ProcessResult> Create(T data, CancellationToken cancellationToken)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await db.Set<T>().AddAsync(data, cancellationToken);
            
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new ProcessResult(true, []);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(e.Message, e);
            
            return new ProcessResult(false, [e.Message]);
        }
    }
    
    public virtual async Task<ProcessResult> CreateBatch(IEnumerable<T> datas, CancellationToken cancellationToken)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await db.Set<T>().AddRangeAsync(datas, cancellationToken);
            
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new ProcessResult(true, []);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(e.Message, e);
            
            return new ProcessResult(false, [e.Message]);
        }
    }
    
    public virtual async Task<ProcessResult> Update(T inputData, CancellationToken cancellationToken)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            if (await db.Set<T>().AnyAsync(x => inputData.Equals(x.Id), cancellationToken))
                throw new Exception($"Data is not found based on the provided id");

            db.Set<T>().Update(inputData);
            
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new ProcessResult(true, []);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(e.Message, e);
            
            return new ProcessResult(false, [e.Message]);
        }
    }
    
    public virtual async Task<ProcessResult> UpdateBatch(IEnumerable<T> inputDatas, CancellationToken cancellationToken)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            inputDatas = inputDatas.ToList();
            
            var ids = inputDatas.Select(x => x.Id);
            if (!await db.Set<T>().AllAsync(x => ids.Contains(x.Id), cancellationToken))
                throw new Exception($"Data is not found based on the provided id");

            foreach (var inputData in inputDatas)
            {
                db.Set<T>().Update(inputData);
            }
            
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new ProcessResult(true, []);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(e.Message, e);
            
            return new ProcessResult(false, [e.Message]);
        }
    }
    
    public virtual async Task<ProcessResult> Delete(long id, CancellationToken cancellationToken)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var data = await db.Set<T>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (data is null)
                throw new Exception($"Data is not found based on the provided id");

            db.Set<T>().Remove(data);
            
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new ProcessResult(true, []);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(e.Message, e);
            
            return new ProcessResult(false, [e.Message]);
        }
    }
    
    public virtual async Task<ProcessResult> DeleteBatch(IEnumerable<long> ids, CancellationToken cancellationToken)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {   
            if (!await db.Set<T>().AllAsync(x => ids.Contains(x.Id), cancellationToken))
                throw new Exception($"Data is not found based on the provided id");

            await db.Set<T>().Where(x => ids.Contains(x.Id)).ExecuteDeleteAsync(cancellationToken);
            
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new ProcessResult(true, []);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(e.Message, e);
            
            return new ProcessResult(false, [e.Message]);
        }
    }
}