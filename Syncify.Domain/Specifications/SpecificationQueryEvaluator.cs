﻿using Microsoft.EntityFrameworkCore;
using Syncify.Domain.Entities;

namespace Syncify.Domain.Specifications;

public static class SpecificationQueryEvaluator
{
    public static IQueryable<TEntity> BuildQuery<TEntity>(
        IQueryable<TEntity> query,
        IBaseSpecification<TEntity> specification) where TEntity : BaseEntity
    {
        var inputQuery = query.AsQueryable();

        if (specification.Includes.Count != 0)
            inputQuery = specification.Includes.Aggregate(inputQuery, (currentQuery, includeExpression) => currentQuery.Include(includeExpression));

        if (specification.Criteria != null)
            inputQuery = inputQuery.Where(specification.Criteria);

        if (specification.IsPagingEnabled)
            inputQuery = inputQuery.Skip((specification.Skip - 1) * specification.Take).Take(specification.Take);

        return inputQuery;

    }
}