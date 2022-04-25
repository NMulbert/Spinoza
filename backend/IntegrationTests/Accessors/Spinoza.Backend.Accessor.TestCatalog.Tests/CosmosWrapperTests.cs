using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Spinoza.Backend.Crosscutting.CosmosDBWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Spinoza.Backend.Accessor.TestCatalog.Tests
{
    class TestItem 
    {
        [JsonProperty("_etag")]
        public string ETag { get; set; }

        [JsonProperty(PropertyName = "Version")]
        public string Version { get; set; } = "1.0.0";

        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "ttl", NullValueHandling = NullValueHandling.Ignore)]
        public int? TTL { get; set; }

        [JsonProperty("Questions")]
        public List<Guid> Questions { get; set; } = new List<Guid>();

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "Descrition")]
        public string Descrition { get; set; } 
        

    }

    public class CosmosWrapperTests
    {
        private readonly ICosmosDBWrapper _cosmosDBWrapper; 
        

        public CosmosWrapperTests(ICosmosDBWrapper cosmosDBWrapper, ICosmosDbInformationProvider cosmosDbInformationProvider )
        {
            Database database = cosmosDBWrapper.CosmosClient.GetDatabase(cosmosDbInformationProvider.DataBaseName);
            try
            {
                database.DeleteAsync().Wait();
            }
            catch(Exception)
            {

            }
            _cosmosDBWrapper = cosmosDBWrapper;
        }


        [Fact]
        public void DBCreationTest()
        {
            var database = _cosmosDBWrapper.Database;
            Assert.NotNull(database);
           
        }
        [Fact]
        public void ContainerCreationTest()
        {
            var testContainer = _cosmosDBWrapper.Container;
            Assert.NotNull(testContainer);

        }
        [Fact]
        public void AlreadyExistDBCreationTest()
        {
            var database = _cosmosDBWrapper.Database;
            Assert.NotNull(database);
            var database2 = _cosmosDBWrapper.Database;
            Assert.NotNull(database2);
            Assert.Equal(database, database2);
        }
        [Fact]
        public void AlreadyExistContainerCreationTest()
        {
            var testContainer = _cosmosDBWrapper.Container;
            Assert.NotNull(testContainer);
            var testContainer2 = _cosmosDBWrapper.Container;
            Assert.NotNull(testContainer2);
            Assert.Equal(testContainer, testContainer2);
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
        [Fact]
        public async void DBGetAllTests()
        {
           // await Task.Delay(2000);
            for (int i = 0; i < 100; i++)
            {
                
                TestItem item = new TestItem()
                {
                    Id = i.ToString(),
                    Title = $"New Test:{i}"

                };
                var result = await _cosmosDBWrapper.CreateItemAsync(item);
            }
            var allItems = await _cosmosDBWrapper.GetAllCosmosElementsAsync<TestItem>(0, 100);
            Assert.NotNull(allItems);
            Assert.Equal("50", allItems[50].Id);
            Assert.Equal(100, allItems.Count);

        }

        [Fact]
        public async void DBGetOffsetWithLimitTests()
        {
            //await Task.Delay(2000);
            for (int i = 0; i < 100; i++)
            {
                
                TestItem item = new TestItem()
                {
                    Id = i.ToString(),
                    Title = $"New Test:{i}"

                };
                var result = await _cosmosDBWrapper.CreateItemAsync(item);
            }
            var allItems = await _cosmosDBWrapper.GetAllCosmosElementsAsync<TestItem>(30, 10);
            Assert.NotNull(allItems);
            Assert.Equal("35", allItems[5].Id);
            Assert.Equal(10, allItems.Count);

        }
        [Fact]
        public async void DBGetSpecificTest()
        {
            var id = "1";
            var query = new QueryDefinition($"SELECT * FROM ITEMS item  WHERE item.id = @id").WithParameter("@id", id);
            //await Task.Delay(2000);
            for (int i = 0; i < 10; i++)
            {

                TestItem item = new TestItem()
                {
                    Id = i.ToString(),
                    Title = $"New Test:{i}"

                };
                var result = await _cosmosDBWrapper.CreateItemAsync(item);
            }
            var allItems = await _cosmosDBWrapper.GetCosmosElementsAsync<TestItem>(query);
            Assert.NotNull(allItems);
            Assert.Equal("1", allItems[0].Id);
            Assert.Equal(1, allItems.Count);
        }

        [Fact]
        public async void DBUpdateTest()
        {

            string id="1";
            //await Task.Delay(2000);
            for (int i = 0; i < 10; i++)
            {

                TestItem item = new TestItem()
                {
                    Id =  Guid.NewGuid().ToString(),
                    Title = $"New Test: {i}"

                };
                id = item.Id.ToString();
                var result = await _cosmosDBWrapper.CreateItemAsync(item);
                
            }
            var query = new QueryDefinition($"SELECT * FROM ITEMS item  WHERE item.id = @id").WithParameter("@id", id);
            
            var newItem = (await _cosmosDBWrapper.GetCosmosElementsAsync<TestItem>(query)).First();
            newItem.Title = "updatedItem";
            newItem.Descrition = "Hello its a test";
            newItem.Questions.Add(new Guid());
            await _cosmosDBWrapper.UpdateItemAsync(newItem, item => item.ETag, item => Guid.Parse((item.Id)), TestMerger);
            var updatedItem = (await _cosmosDBWrapper.GetCosmosElementsAsync<TestItem>(query)).First();
            
            Assert.Equal(newItem.Id, updatedItem.Id);
            Assert.Equal("updatedItem", updatedItem.Title);
            Assert.Equal(1, updatedItem.Questions.Count);
            Assert.Equal("Hello its a test", updatedItem.Descrition);
        }
        private TestItem TestMerger(TestItem dbItem, TestItem newItem)
        {
            dbItem.Descrition = newItem.Descrition;
            dbItem.Questions = dbItem.Questions.Union(newItem.Questions).ToList();
            dbItem.Title = newItem.Title;
            return dbItem;
        }
        [Fact]
        public async void DBUpdateTestIfAlreadyExist()
        {

            string id = "1";
            //await Task.Delay(2000);
            for (int i = 0; i < 10; i++)
            {

                TestItem item = new TestItem()
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = $"New Test: {i}"

                };
                id = item.Id.ToString();
                var result = await _cosmosDBWrapper.CreateItemAsync(item);

            }
            
            var query = new QueryDefinition($"SELECT * FROM ITEMS item  WHERE item.id = @id").WithParameter("@id", id);

            var newItem = (await _cosmosDBWrapper.GetCosmosElementsAsync<TestItem>(query)).First();
            newItem.Title = "updatedItem";
            newItem.Descrition = "Hello its a test";
            newItem.Questions.Add(new Guid());
            int counter = 0;
            var etag = newItem.ETag;
            await _cosmosDBWrapper.UpdateItemAsync(newItem, item => item.ETag, item => Guid.Parse((item.Id)), TestMerger);
            var updatedItem = (await _cosmosDBWrapper.GetCosmosElementsAsync<TestItem>(query)).First();
            newItem.Title = "updatedItem2";
            await _cosmosDBWrapper.UpdateItemAsync(newItem, item => item.ETag, item => Guid.Parse((item.Id)), TestMergerRetry);
            var updatedItem2 = (await _cosmosDBWrapper.GetCosmosElementsAsync<TestItem>(query)).First();
            
            Assert.Equal(newItem.Id, updatedItem.Id);
            Assert.Equal("updatedItem", updatedItem.Title);
            Assert.Equal(2, counter);
            Assert.Equal("updatedItem2", updatedItem2.Title);
            Assert.Equal("2", updatedItem2.Descrition);


            TestItem TestMerger(TestItem dbItem, TestItem newItem)
            {
                dbItem.Descrition = newItem.Descrition;
                dbItem.Questions = dbItem.Questions.Union(newItem.Questions).ToList();
                dbItem.Title = newItem.Title;
                return dbItem;
            }
            TestItem TestMergerRetry(TestItem dbItem, TestItem newItem)
            {
                counter++;
                if(counter == 1)//first time
                {
                    dbItem.ETag = etag;
                }
                dbItem.Descrition = counter.ToString();
                dbItem.Questions = dbItem.Questions.Union(newItem.Questions).ToList();
                dbItem.Title = newItem.Title;
                return dbItem;
            }
        }
      
        
    }
}