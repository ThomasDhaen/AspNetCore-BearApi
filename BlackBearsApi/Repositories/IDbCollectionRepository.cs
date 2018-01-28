using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlackBearsApi.Repositories
{
    public interface IDbCollectionRepository<TModel, in TParam>
    {
        Task<IEnumerable<TModel>> GetItemsFromCollectionAsync();
        Task<TModel> GetItemFromCollectionAsync(TParam id);
        Task<TModel> AddDocumentIntoCollectionAsync(TModel item);
        Task<TModel> UpdateDocumentFromCollection(TParam id, TModel item);
        Task DeleteDocumentFromCollectionAsync(TParam id);
    }
}
