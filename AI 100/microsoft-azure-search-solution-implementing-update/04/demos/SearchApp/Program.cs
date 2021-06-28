using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Azure.Search.Documents.Indexes.Models;

using System;
using System.Collections.Generic;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace SearchApp
{
    class Program
    {
        private static string queryKey = "<key>";
        private static string adminKey = "<key>";
        private static Uri endpoint = new Uri("https://[instancename].search.windows.net/");
        private static string indexName = "customers";

        static void Main(string[] args)
        {
            // CreateIndex();
            // AddDocuments();
            SearchDocuments();
        }

        private static void SearchDocuments()
        {
            // Create a client
            AzureKeyCredential credential = new AzureKeyCredential(queryKey);
            var client = new Azure.Search.Documents.Indexes.SearchIndexClient(endpoint, credential);
            var searchClient = client.GetSearchClient("customers");
            var response = searchClient.Search<Customer>("satya").Value;
            foreach (Azure.Search.Documents.Models.SearchResult<Customer> result in response.GetResults())
            {
                Console.WriteLine($"Score:{result.Score}, Id:{result.Document.Id}, Name:{result.Document.Name}");
            }
        }

        private static void AddDocuments()
        {
            var actions = new IndexAction<Customer>[]
            {
                IndexAction.Upload(new Customer { Id = "1", Name = "Satya Nadella" }),
                IndexAction.Upload(new Customer { Id = "2", Name = "Scott Guthrie" })
            };
            var batch = IndexBatch.New(actions);
            SearchServiceClient searchServiceClient = 
                new SearchServiceClient("sahilsearch", new SearchCredentials(adminKey));
            var indexClient = searchServiceClient.Indexes.GetClient(indexName);
            indexClient.Documents.Index(batch);
        }

        private static void CreateIndex()
        {

            AzureKeyCredential credential = new AzureKeyCredential(adminKey);
            var client = new Azure.Search.Documents.Indexes.SearchIndexClient(endpoint, credential);
            List<SearchField> fields = new List<SearchField>();

            var idField = new SearchField("Id", SearchFieldDataType.String);
            idField.IsSearchable = false;
            idField.IsKey = true;
            fields.Add(idField);

            var nameField = new SearchField("Name", SearchFieldDataType.String);
            nameField.IsSearchable = true;
            nameField.IsHidden = false;
            fields.Add(nameField);

            client.CreateIndex(new SearchIndex(indexName, fields));
        }
    }

    public class Customer
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
