﻿using Syncify.Domain.Entities;
using System.Linq.Expressions;

namespace Syncify.Domain.Specifications;
public abstract class BaseSpecification<TEntity> : IBaseSpecification<TEntity> where TEntity : BaseEntity
{
    public List<Expression<Func<TEntity, object>>> Includes { get; set; } = [];
    public List<Expression<Func<TEntity, object>>> OrderBy { get; set; } = [];
    public List<Expression<Func<TEntity, object>>> OrderByDescending { get; set; } = [];
    public Expression<Func<TEntity, bool>>? Criteria { get; set; }
    public bool IsTracking { get; set; } = true;
    public bool IsPagingEnabled { get; set; }
    public int Skip { get; set; }
    public int Take { get; set; }

    protected BaseSpecification() { }

    protected BaseSpecification(Expression<Func<TEntity, bool>> criteriaExpression)
        => Criteria = criteriaExpression;

    protected void AddIncludes(Expression<Func<TEntity, object>> includeExpression)
        => Includes.Add(includeExpression);

    protected void AddOrderBy(Expression<Func<TEntity, object>> orderByExpression)
       => OrderBy.Add(orderByExpression);

    protected void AddOrderByDescending(Expression<Func<TEntity, object>> orderByDescendingExpression)
        => OrderByDescending.Add(orderByDescendingExpression);

    protected void DisableTracking() => IsTracking = false;


}
