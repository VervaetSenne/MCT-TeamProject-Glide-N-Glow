namespace GlideNGlow.Core.Services.Abstractions;

public interface IBaseService<T>
{
    Task<IEnumerable<T>> FindAsync();
    Task<T> CreateAsync(T entity);
}