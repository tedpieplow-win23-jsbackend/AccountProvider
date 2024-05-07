using Infrastructure.Contexts;
using Infrastructure.Factories;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repositories;

public abstract class BaseRepo<TEntity>(DataContext context) where TEntity : class
{
    private readonly DataContext _context = context;

    public virtual async Task<ResponseResult> CreateOneAsync(TEntity entity)
    {
		try
		{
            _context.Set<TEntity>().Add(entity);
            await _context.SaveChangesAsync();
            return ResponseFactory.Ok(entity);
        }
		catch (Exception ex)
		{

            return ResponseFactory.Error(ex.Message);
		}
    }

    public virtual async Task<ResponseResult> GetAllAsync()
    {
        try
        {
            IEnumerable<TEntity> list = await _context.Set<TEntity>().ToListAsync();

            if (!list.Any())
                return ResponseFactory.NotFound("List is empty.");

            return ResponseFactory.Ok(list);
        }
        catch (Exception ex)
        {

            return ResponseFactory.Error(ex.Message);
        }
    }

    public virtual async Task<ResponseResult> GetOneAsync(Expression<Func<TEntity, bool>> predicate)
    {
        try
        {
            var entity = await _context.Set<TEntity>().FirstOrDefaultAsync(predicate);

            if (entity == null)
                return ResponseFactory.NotFound();
            
            return ResponseFactory.Ok(entity);
        }
        catch (Exception ex)
        {
            return ResponseFactory.Error(ex.Message);
        }
    }

    public virtual async Task<ResponseResult> UpdateAsync(TEntity entity)
    {
        try
        {
            _context.Set<TEntity>().Update(entity);
            await _context.SaveChangesAsync();
            return ResponseFactory.Ok(entity, "Successfully updated.");
        }
        catch (Exception ex)
        {

            return ResponseFactory.Error(ex.Message);
        }
    }

    public virtual async Task<ResponseResult> UpdateAsync(TEntity entity, Expression<Func<TEntity, bool>> predicate)
    {
        try
        {
            var existResult = await ExistsAsync(predicate);

            if (existResult.StatusCode == StatusCode.EXISTS)
            {
                _context.Set<TEntity>().Update(entity);
                await _context.SaveChangesAsync();
                return ResponseFactory.Ok(entity, "Successfully updated.");
            }

            else if (existResult.StatusCode == StatusCode.NOT_FOUND)
                return ResponseFactory.NotFound();

            return ResponseFactory.Error();
        }
        catch (Exception ex)
        {
            return ResponseFactory.Error(ex.Message);
        }
    }

    public virtual async Task<ResponseResult> DeleteAsync(TEntity entity)
    {
        try
        {
            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();
            return ResponseFactory.Ok("Successfully deleted");
        }
        catch (Exception ex)
        {
            return ResponseFactory.Error(ex.Message);
        }
    }

    public virtual async Task<ResponseResult> DeleteAsync(TEntity entity, Expression<Func<TEntity, bool>> predicate)
    {
        try
        {
            var result = await GetOneAsync(predicate);

            if (result.StatusCode == StatusCode.OK)
            {
                _context.Set<TEntity>().Remove(entity);
                await _context.SaveChangesAsync();
                return ResponseFactory.Ok("Successfully deleted");
            }

            else if (result.StatusCode == StatusCode.NOT_FOUND)
                return ResponseFactory.NotFound();

            return ResponseFactory.Error();
        }
        catch (Exception ex)
        {
            return ResponseFactory.Error(ex.Message);
        }
    }

    public virtual async Task<ResponseResult> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
    {
        try
        {
            var existResult = await _context.Set<TEntity>().AnyAsync(predicate);

            if (existResult)
                return ResponseFactory.Exists();

            return ResponseFactory.NotFound();
        }
        catch (Exception ex)
        {
            return ResponseFactory.Error(ex.Message);
        }
    }
}
