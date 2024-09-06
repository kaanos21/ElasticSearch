using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using ElasticSearch.Web.Model;
using ElasticSearch.Web.ViewMoels;

namespace ElasticSearch.Web.Repository
{
    public class ECommerceRepository
    {
        private readonly ElasticsearchClient _elasticsearchClient;
        private const string indexName = "kibana_sample_data_ecommerce";

        public ECommerceRepository(ElasticsearchClient elasticsearchClient)
        {
            _elasticsearchClient = elasticsearchClient;
        }

        public async Task<(List<ECommerce> list , long count)> SearchAsync(ECommerceSeachViewModel seachViewModel,int page,int pageSize)
        {

            List<Action<QueryDescriptor<ECommerce>>> ListQuery = new List<Action<QueryDescriptor<ECommerce>>>();
            if (seachViewModel is null)
            {
                ListQuery.Add(g => g.MatchAll());
                return await CalculateResultSet(page, pageSize, ListQuery);
            }
            if (!string.IsNullOrEmpty(seachViewModel.Category))
            {
                ListQuery.Add((q => q.Match(m => m.Field(f => f.Category).Query(seachViewModel.Category))));
            }
            if (!string.IsNullOrEmpty(seachViewModel.CustomerFullName))
            {
                ListQuery.Add((q => q.Match(m => m.Field(f => f.CustomerFullName).Query(seachViewModel.CustomerFullName))));
            }
            if (seachViewModel.OrderDateStart.HasValue)
            {
                ListQuery.Add((q => q.Range(r => r.DateRange(dr => dr.Field(f => f.OrderDate).Gte(seachViewModel.OrderDateStart.Value)))));
            }
            if (seachViewModel.OrderDateEnd.HasValue)
            {
                ListQuery.Add((q => q.Range(r => r.DateRange(dr => dr.Field(f => f.OrderDate).Lte(seachViewModel.OrderDateEnd.Value)))));
            }
            if (string.IsNullOrEmpty(seachViewModel.Gender))
            {
                ListQuery.Add(q => q.Term(t => t.Field(f => f.Gender).Value(seachViewModel.Gender)));
            }
            if (!ListQuery.Any())
            {
                ListQuery.Add(q => q.Term(t => t.Field(f => f.Gender).Value(seachViewModel.Gender)));
            }
            return await CalculateResultSet(page, pageSize, ListQuery);
        }

        private async Task<(List<ECommerce> list, long count)> CalculateResultSet(int page, int pageSize, List<Action<QueryDescriptor<ECommerce>>> ListQuery)
        {
            var pageFrom = (page - 1) * pageSize;
            var result = await _elasticsearchClient.SearchAsync<ECommerce>(s => s.Index(indexName).Size(pageSize).From(pageFrom).Query(q => q.Bool(b => b.Must(ListQuery.ToArray()))));
            foreach (var hit in result.Hits) { hit.Source.Id = hit.Id; }
            return (list: result.Documents.ToList(), result.Total);
        }
    }
}
