using System;
using System.Collections.Generic;
using System.Linq;

namespace TellMe.Web.DAL.Extensions
{
    public static class CollectionExtensions
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            foreach (var element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static void MapFrom<TDto, TEntity>(this ICollection<TEntity> destination, ICollection<TDto> source,
            Func<ICollection<TDto>, TEntity, bool> deletedPredicate,
            Func<ICollection<TEntity>, TDto, bool> newEntityPredicate,
            Action<TDto, TEntity> map) where TEntity : new()
        {
            var deletedResources = destination.Where(entity => deletedPredicate(source, entity)).ToList();
            var newResources = source.Where(resourceId => newEntityPredicate(destination, resourceId))
                .Select(resourceId =>
                {
                    var entity = new TEntity();
                    map(resourceId, entity);
                    return entity;
                }).ToList();

            foreach (var item in deletedResources)
            {
                destination.Remove(item);
            }

            foreach (var item in newResources)
            {
                destination.Add(item);
            }
        }

        public static void MapFrom<TDto, TEntity>(this ICollection<TEntity> destination, ICollection<TDto> source,
            Func<ICollection<TDto>, TEntity, bool> deletedPredicate,
            Func<ICollection<TEntity>, TDto, bool> newEntityPredicate,
            Func<TEntity, TDto, bool> equalityComparer,
            Action<TDto, TEntity> map) where TEntity : new()
        {
            foreach (var entity in destination)
            {
                var dto = source.FirstOrDefault(x => equalityComparer(entity, x));
                map(dto, entity);
            }

            destination.MapFrom(source, deletedPredicate, newEntityPredicate, map);
        }

        public static void MapFrom<TDto, TEntity, TId>(this ICollection<TEntity> destination, ICollection<TDto> source,
            Func<TDto, TId> dtoIdFunc,
            Func<TEntity, TId> entityIdFunc,
            Action<TDto, TEntity> map) where TEntity : new()
        {
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var existingEntities = (from entity in destination
                join dto in source on entityIdFunc(entity) equals dtoIdFunc(dto) into g
                from x in g.DefaultIfEmpty()
                select new {entity, x}).ToList();

            var newEntities = (from dto in source
                join entity in destination on dtoIdFunc(dto) equals entityIdFunc(entity) into g
                from x in g.DefaultIfEmpty()
                where x == null
                select dto).ToList();


            foreach (var item in existingEntities)
            {
                if (Equals(item.x, default(TDto)))
                    destination.Remove(item.entity);
                else
                    map(item.x, item.entity);
            }

            foreach (var item in newEntities)
            {
                var entity = new TEntity();
                map(item, entity);
                destination.Add(entity);
            }
        }
    }
}