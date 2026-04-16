using Microsoft.Azure.Cosmos;
using CosmosDB_Demo.Models;

namespace CosmosDB_Demo.Data
{
    public class CosmosDbService
    {
        private Container _container;

        public CosmosDbService(CosmosClient cosmosClient,
            string databaseName, string containerName)
        {
            _container = cosmosClient.GetContainer(databaseName, containerName);
        }

        // ADD ITEM
        public async Task AddItemAsync(Itemmodel item)
        {
            await _container.CreateItemAsync(item, new PartitionKey(item.Id));
        }

        // GET SINGLE ITEM
        public async Task<Itemmodel> GetItemAsync(string id)
        {
            ItemResponse<Itemmodel> response = await
                _container.ReadItemAsync<Itemmodel>(id, new PartitionKey(id));

            return response.Resource;
        }

        // GET ALL ITEMS (WITH QUERY STRING)
        public async Task<IEnumerable<Itemmodel>> GetItemsAsync(string queryString)
        {
            var query = _container.GetItemQueryIterator<Itemmodel>(
                new QueryDefinition(queryString));

            List<Itemmodel> results = new List<Itemmodel>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        // UPDATE ITEM
        public async Task UpdateItemAsync(string id, Itemmodel item)
        {
            // ensure id consistency
            item.Id = id;

            await _container.UpsertItemAsync(item, new PartitionKey(id));
        }

        // DELETE ITEM
        public async Task DeleteItemAsync(string id)
        {
            await _container.DeleteItemAsync<Itemmodel>(id, new PartitionKey(id));
        }
    }
}