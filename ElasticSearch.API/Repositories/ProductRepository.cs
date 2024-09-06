﻿using Elastic.Clients.Elasticsearch;
using ElasticSearch.API.DTOs;
using ElasticSearch.API.Models;
using System.Collections.Immutable;

namespace ElasticSearch.API.Repositories
{
    public class ProductRepository
    {
        private readonly ElasticsearchClient _client;
        private const string indexName = "products";
        public ProductRepository(ElasticsearchClient client)
        {
            _client = client;
        }
        public async Task<Product?> SaveAsync(Product newProduct)
        {
            newProduct.Created = DateTime.Now;

            var response = await _client.IndexAsync(newProduct, i => i
                .Index(indexName) // Belgeye bir ID belirtilmeden eklenir
            );

            if (!response.IsSuccess())
            {
                Console.WriteLine($"Error indexing document: {response.ElasticsearchServerError?.Error?.Reason}");
                return null;
            }

            // Belgenin ID'sini al ve güncelle
            newProduct.Id = response.Id;
            return newProduct;
        }

        public async Task<ImmutableList<Product>> GetAllAsync()
        {
            var result = await _client.SearchAsync<Product>(s => s
                .Index(indexName)
                .Query(q => q.MatchAll(_ => { })) // MatchAll kullanımı burada parametre bekliyor olabilir
            );
            foreach (var hit in result.Hits)
            {
                hit.Source.Id = hit.Id;
            }
            return result.Documents.ToImmutableList();
        }
        public async Task<Product?> GetByIdAsync(string id)
        {
            var response=await _client.GetAsync<Product>(id,x=>x.Index(indexName));

            if(!response.IsSuccess()) { return null; }

            response.Source.Id=response.Id;

            return response.Source;
        }
        public async Task<bool> UpdateAsync(ProductUpdateDto updateProduct)
        {
            var response = await _client.UpdateAsync<Product, ProductUpdateDto>(indexName,updateProduct.Id,x=>x.Doc(updateProduct));
            return response.IsSuccess();
        }
        public async Task<DeleteResponse> DeleteAsync(string id)
        {
            var response=await _client.DeleteAsync<Product>(id,x=>x.Index(indexName));
            return response;
        }
    }
}
