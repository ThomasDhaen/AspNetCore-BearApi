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
    public class DbCollectionRepository : IDbCollectionRepository<Bear, string>
    {

        private static readonly string Endpoint = "https://YOURDB.documents.azure.com:443/";
        private static readonly string Key = "FirstKey";
        private static readonly string DatabaseId = "BlackBearsDB";
        private static readonly string CollectionId = "BearCollection";
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

        public async Task<Bear> AddDocumentIntoCollectionAsync(Bear item)
        {
            try
            {
                var document = await docClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), item);
                var res = document.Resource;
                var bear = JsonConvert.DeserializeObject<Bear>(res.ToString());
                return bear;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteDocumentFromCollectionAsync(string name)
        {
            await docClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, name));
        }

        public async Task<Bear> GetItemFromCollectionAsync(string name)
        {
            try
            {
                Document doc = await docClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, name));
                return JsonConvert.DeserializeObject<Bear>(doc.ToString());
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<IEnumerable<Bear>> GetItemsFromCollectionAsync()
        {
            var bears = docClient.CreateDocumentQuery<Bear>(
                  UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                  new FeedOptions { MaxItemCount = -1 })
                  .AsDocumentQuery();
            List<Bear> persons = new List<Bear>();
            while (bears.HasMoreResults)
            {
                persons.AddRange(await bears.ExecuteNextAsync<Bear>());
            }
            return persons;
        }

        public async Task<Bear> UpdateDocumentFromCollection(string name, Bear item)
        {
            try
            {
                var document = await docClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, name), item);
                var data = document.Resource.ToString();
                var person = JsonConvert.DeserializeObject<Bear>(data);
                return person;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
