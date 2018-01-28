using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlackBearsApi.Repositories
{
    public interface IDbCollectionRepository<TModel>
    {
        Task<IEnumerable<TModel>> GetItemsFromCollectionAsync();
        Task<TModel> GetItemFromCollectionAsync(string id);
        Task<TModel> AddDocumentIntoCollectionAsync(TModel item);
        Task<TModel> UpdateDocumentFromCollection(string id, TModel item);
        Task DeleteDocumentFromCollectionAsync(string id);
    }
}
