using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using ElasticSearch.API.Models.EcommerceModel;
using System.Collections.Immutable;

namespace ElasticSearch.API.Repositories
{
    public class ECommerceRepository
    {

        private readonly ElasticsearchClient _client;
        private const string indexName = "kibana_sample_data_ecommerce";

        public ECommerceRepository(ElasticsearchClient client)
        {
            _client = client;
        }

        public async Task<ImmutableList<ECommerce>> TermQuery(string customerFirstName)
        {
            //1. way
            //var result=await _client.SearchAsync<ECommerce>(s=>s.Index(indexName).Query(q=>q.Term(t=>t.Field("customer_first_name.keyword").Value(customerFirstName))));
            //2. way
            //var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Query(q => q.Term(t => t.CustomerFirstName.Suffix("keyword"), customerFirstName)));
            //3. way
            var termQuery = new TermQuery("customer_first_name.keyword") { Value = customerFirstName, CaseInsensitive = true };
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Query(termQuery));
            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;
            return result.Documents.ToImmutableList();
        }
        public async Task<ImmutableList<ECommerce>> TermsQuery(List<String> customerFirstNameList)
        {
            List<FieldValue> terms = new List<FieldValue>();
            customerFirstNameList.ForEach(x =>
            {
                terms.Add(x);
            });
            //1. way
            //var termsQuery = new TermsQuery()
            //{
            //    Field = "customer_first_name.keyword",
            //    Terms = new TermsQueryField(terms.AsReadOnly())
            //};
            //var result=await _client.SearchAsync<ECommerce>(s=>s.Index(indexName).Query(termsQuery));

            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Query(q => q.Terms(t => t.Field(f => f.CustomerFirstName.Suffix("keyword")).Terms(new TermsQueryField(terms.AsReadOnly())))));
            foreach (var hit in result.Hits)
            {
                hit.Source.Id = hit.Id;
            }




            return result.Documents.ToImmutableList();
        }
        public async Task<ImmutableList<ECommerce>> PrefixQueryAsync(string CustomerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Query(q => q.Prefix(p => p.Field(f => f.CustomerFullName.Suffix("keyword")).Value(CustomerFullName))));
            return result.Documents.ToImmutableList();

        }
        public async Task<ImmutableList<ECommerce>> RangeQueryAsync(double FromPrice, double ToPrice)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
                .Query(q => q
                .Range(r => r.NumberRange(nr => nr.Field(f => f.TaxfulTotalPrice).Gte(FromPrice).Lte(ToPrice)))));
            return result.Documents.ToImmutableList();
        }
        public async Task<ImmutableList<ECommerce>> MatchAllQueryAsync()
        {
            var result = await _client.SearchAsync<ECommerce>(s =>
                s.Index(indexName)
                 .Size(100)
                 .Query(q => q.MatchAll())
            );
            return result.Documents.ToImmutableList();
        }
        public async Task<ImmutableList<ECommerce>> PaginationQueryAsync(int page,int pagesize)
        {

            var pagefrom=(page-1)*pagesize;

            var result = await _client.SearchAsync<ECommerce>(s =>
                s.Index(indexName)
                 .Size(pagesize).From(pagefrom)
                 .Query(q => q.MatchAll())
            );
            return result.Documents.ToImmutableList();
        }
        public async Task<ImmutableList<ECommerce>> WildCardQueryAsync(string CustomerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s =>
                s.Index(indexName)
                 .Query(q => q.Wildcard(w => w.Field(f => f.CustomerFullName.Suffix("keyword")).Wildcard(CustomerFullName)))
            );
            return result.Documents.ToImmutableList();
        }
        public async Task<ImmutableList<ECommerce>> FuzzyQueryAsync(string CustomerName)
        {
            var result = await _client.SearchAsync<ECommerce>(s =>
                s.Index(indexName)
                 .Query(q => q.Fuzzy(fu => fu.Field(f => f.CustomerFirstName.Suffix("keyword")).Value(CustomerName).Fuzziness(new Fuzziness(1)))).Sort(sort => sort.Field(f => f.TaxfulTotalPrice, new FieldSort() { Order = SortOrder.Desc })));

            return result.Documents.ToImmutableList();
        }
        public async Task<ImmutableList<ECommerce>> MatchBoolPrefixFullTextQueryAsync(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s =>
                s.Index(indexName)
                 .Query(q => q.MatchBoolPrefix(m => m.Field(f => f.CustomerFullName).Query(customerFullName))));
            foreach (var hit in result.Hits) { hit.Source.Id=hit.Id; }
            return result.Documents.ToImmutableList();
        }
        public async Task<ImmutableList<ECommerce>> MatchPhraseFullTextQueryAsync(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s =>
                s.Index(indexName)
                 .Query(q => q.MatchPhrase(m => m.Field(f => f.CustomerFullName).Query(customerFullName))));
            foreach (var hit in result.Hits) { hit.Source.Id = hit.Id; }
            return result.Documents.ToImmutableList();
        }
        public async Task<ImmutableList<ECommerce>> CompoundQueryExampleOneAsync(string cityName,double taxfulTotalPrice,string categoryName,string menufacturer)
        {
            var result = await _client.SearchAsync<ECommerce>(s =>
                s.Index(indexName)
                 .Query(q => q.Bool(b => b.Must(m => m.Term(t => t.Field("geoip.city_name").Value(cityName))).MustNot(mn => mn.Range(r => r.NumberRange(nr => nr.Field(f => f.TaxfulTotalPrice).Lte(taxfulTotalPrice)))).Should(s=>s.Term(t=>t.Field(f=>f.Category.Suffix("keyword")).Value(categoryName))).Filter(f=>f.Term(t=>t.Field("manufacturer.keyword").Value(menufacturer))))));
            foreach (var hit in result.Hits) { hit.Source.Id = hit.Id; }
            return result.Documents.ToImmutableList();
        }
    }
}
