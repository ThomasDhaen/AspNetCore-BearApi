using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlackBearApi.Model;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;

namespace BlackBearsApi.Repositories
{
    public class DbCollectionRepository<TModel> : IDbCollectionRepository<TModel>
    {

        private static readonly string Endpoint = "https://YOURDB.documents.azure.com:443/";
        private static readonly string Key = "FirstKey";
        private static readonly string DatabaseId = "BlackBearsDB";
        private static readonly string CollectionId = $"{typeof(TModel).Name}Collection";
        private static DocumentClient docClient;

        public DbCollectionRepository()
        {
            docClient = new DocumentClient(new Uri(Endpoint), Key);
            CreateDatabaseIfNotExistsAsync().Wait();
            CreateCollectionIfNotExistsAsync().Wait();
        }

        private static async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await docClient.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(DatabaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await docClient.CreateDatabaseAsync(new Database { Id = DatabaseId });
                }
                else
                {
                    throw;
                }
            }
        }

        private static async Task CreateCollectionIfNotExistsAsync()
        {
            try
            {
                await docClient.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await docClient.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(DatabaseId),
                        new DocumentCollection { Id = CollectionId },
                        new RequestOptions { OfferThroughput = 1000 });
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<TModel> AddDocumentIntoCollectionAsync(TModel item)
        {
            try
            {
                var document = await docClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), item);
                var res = document.Resource;
                var json = JsonConvert.DeserializeObject<TModel>(res.ToString());
                return json;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteDocumentFromCollectionAsync(string id)
        {
            await docClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));
        }

        public async Task<TModel> GetItemFromCollectionAsync(string id)
        {
            try
            {
                Document doc = await docClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));
                return JsonConvert.DeserializeObject<TModel>(doc.ToString());
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return default(TModel);
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<IEnumerable<TModel>> GetItemsFromCollectionAsync()
        {
            var items = docClient.CreateDocumentQuery<TModel>(
                  UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                  new FeedOptions { MaxItemCount = -1 })
                  .AsDocumentQuery();
            List<TModel> persons = new List<TModel>();
            while (items.HasMoreResults)
            {
                persons.AddRange(await items.ExecuteNextAsync<TModel>());
            }
            return persons;
        }

        public async Task<TModel> UpdateDocumentFromCollection(string id, TModel item)
        {
            try
            {
                var document = await docClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id), item);
                var data = document.Resource.ToString();
                var json = JsonConvert.DeserializeObject<TModel>(data);
                return json;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
