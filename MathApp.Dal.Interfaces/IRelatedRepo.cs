using System.Linq.Expressions;

namespace MathApp.Dal.Interfaces;

public interface IRelatedRepo<T> : IRepo<T> where T : new()
{
    // Given an entity e with member m we do:
    // repo.LoadMemberAsync(e, e => e.m)
    Task LoadMemberAsync<TProperty>(T entity, Expression<Func<T, TProperty?>> selector)
        where TProperty : class;
    // Given an entity e with collection property c we do:
    // repo.LoadCollectionAsync(e, e => e.c)
    Task LoadCollectionAsync<TProperty>(T entity, Expression<Func<T, IEnumerable<TProperty>>> selector)
        where TProperty : class;
    Task<int> CountCollectionAsync<TProperty>(T entity,
        Expression<Func<T, IEnumerable<TProperty>>> selector)
        where TProperty : class;
}