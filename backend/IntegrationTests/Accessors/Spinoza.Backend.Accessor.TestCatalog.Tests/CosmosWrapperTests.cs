using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Spinoza.Backend.Accessor.TestCatalog.DataBases;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Spinoza.Backend.Accessor.TestCatalog.Tests
{
    class TestItem 
    {
        [JsonProperty("_etag")]
        public string ETag { get; set; }
        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "ttl", NullValueHandling = NullValueHandling.Ignore)]
        public int? TTL { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
        
    }

    public class CosmosWrapperTests
    {
        private readonly ICosmosDBWrapper _cosmosDBWrapper; 

        public CosmosWrapperTests(ICosmosDBWrapper cosmosDBWrapper )
        {
            Database database = cosmosDBWrapper.CosmosClient.GetDatabase("Catalog");
            try
            {
                Task.Run(() => database.DeleteAsync()).Wait();
            }
            catch(Exception)
            {

            }
            _cosmosDBWrapper = cosmosDBWrapper;
        }


        [Fact]
        public void DBCreationTest()
        {
            var database = _cosmosDBWrapper.TestDatabase;
            Assert.NotNull(database);
           
        }
        [Fact]
        public void ContainerCreationTest()
        {
            var testContainer = _cosmosDBWrapper.TestContainer;
            Assert.NotNull(testContainer);

        }
        [Fact]
        public void AlreadyExistDBCreationTest()
        {
            var database = _cosmosDBWrapper.TestDatabase;
            Assert.NotNull(database);
            database = _cosmosDBWrapper.TestDatabase;
            Assert.NotNull(database);
        }
        [Fact]
        public void AlreadyExistContainerCreationTest()
        {
            var testContainer = _cosmosDBWrapper.TestContainer;
            Assert.NotNull(testContainer);
            testContainer = _cosmosDBWrapper.TestContainer;
            Assert.NotNull(testContainer);
        }

        [Fact]
        public async void DBCreateItemTest()
        {
            TestItem item = new TestItem()
            {
                Id = "1234",
                Title = "New Test",

            };
            var result = await _cosmosDBWrapper.CreateItemAsync(item);
            Assert.NotNull(result);
            Assert.Equal("Created",result.StatusCode.ToString());
            
        }
    }
}