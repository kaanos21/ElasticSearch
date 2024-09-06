using Elastic.Clients.Elasticsearch;
using ElasticSearch.Web.Model;

namespace ElasticSearch.Web.Repository
{
    public class BlogRepository
    {
        private readonly ElasticsearchClient _elasticsearchClient;
        private const string indexName = "blog";
        public BlogRepository(ElasticsearchClient elasticsearchClient)
        {
            _elasticsearchClient = elasticsearchClient;
        }

        public async Task<Blog?> SaveAsync(Blog newBlog)
        {
            newBlog.Created = DateTime.Now;

            var response = await _elasticsearchClient.IndexAsync(newBlog, i => i
                .Index(indexName) // Belgeye bir ID belirtilmeden eklenir
            );
            if (!response.IsValidResponse)
            {
                Console.WriteLine($"Error indexing document: {response.ElasticsearchServerError?.Error?.Reason}");
                return null;
            }
            // Belgenin ID'sini al ve güncelle
            newBlog.Id = response.Id;
            return newBlog;
        }
        public async Task<List<Blog>> SearchAsync(string searchText)
        {
            var result=await _elasticsearchClient.SearchAsync<Blog>(s=>s.Index(indexName).Size(1000).Query(q=>q.Bool(b=>b.Should(m=>m.Match(m=>m.Field(f=>f.Content).Query(searchText)).MatchBoolPrefix(p=>p.Field(f=>f.Title).Query(searchText))))));

            foreach (var hit in result.Hits) { hit.Source.Id = hit.Id; }
            return result.Documents.ToList();
        }
     }
}
