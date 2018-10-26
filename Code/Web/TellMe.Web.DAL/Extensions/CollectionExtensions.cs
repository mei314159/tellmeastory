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

        public static void MapFrom<TDto, TEntity, TId>(this ICollection<TEntity> destination, ICollection<TDto> source,
            Func<TDto, TId> dtoIdFunc,
            Func<TEntity, TId> entityIdFunc,
            Action<TDto, TEntity> map,
            Func<TEntity, TDto, bool> skipFunc = null) where TEntity : new()
        {
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var existingEntities = (from entity in destination
                                    join dto in source on entityIdFunc(entity) equals dtoIdFunc(dto) into g
                                    from x in g.DefaultIfEmpty()
                                    select new { entity, x }).ToList();

            var newEntities = (from dto in source
                               join entity in destination on dtoIdFunc(dto) equals entityIdFunc(entity) into g
                               from x in g.DefaultIfEmpty()
                               where x == null
                               select dto).ToList();


            foreach (var item in existingEntities)
            {
                if (skipFunc?.Invoke(item.entity, item.x) != true)
                {
                    if (Equals(item.x, default(TDto)))
                        destination.Remove(item.entity);
                    else
                        map(item.x, item.entity);
                }
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