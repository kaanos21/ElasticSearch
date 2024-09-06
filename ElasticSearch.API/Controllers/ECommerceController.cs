using ElasticSearch.API.Models.EcommerceModel;
using ElasticSearch.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;

namespace ElasticSearch.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ECommerceController : ControllerBase
    {
        private readonly ECommerceRepository _repository;
        public ECommerceController(ECommerceRepository repository)
        {
            _repository = repository;
        }
        [HttpGet]
        public async Task<IActionResult> TermQuery(string customerFirstName)
        {
            return Ok(await _repository.TermQuery(customerFirstName));
        }
        [HttpPost]
        public async Task<IActionResult> TermsQuery(List<string> customerFirstName)
        {
            return Ok(await _repository.TermsQuery(customerFirstName));
        }
        [HttpGet]
        public async Task<IActionResult> PrefixQuery(string customerFullName)
        {
            return Ok(await _repository.PrefixQueryAsync(customerFullName));
        }
        [HttpGet]
        public async Task<IActionResult> RangeQuery(double FromPrice, double ToPrice)
        {
            return Ok(await _repository.RangeQueryAsync(FromPrice, ToPrice));
        }
        [HttpGet]
        public async Task<IActionResult> MatchAllQuery()
        {
            return Ok(await _repository.MatchAllQueryAsync());
        }
        [HttpGet]
        public async Task<IActionResult> PaginationQueryAsync(int page, int pagesize)
        {
            return Ok(await _repository.PaginationQueryAsync(page, pagesize));
        }
        [HttpGet]
        public async Task<IActionResult> WildCardQueryAsync(string CustomerFullName)
        {
            return Ok(await _repository.WildCardQueryAsync(CustomerFullName));
        }
        [HttpGet]
        public async Task<IActionResult> FuzzyQueryAsync(string CustomerName)
        {
            return Ok(await _repository.FuzzyQueryAsync(CustomerName));
        }
        [HttpGet]
        public async Task<IActionResult> MatchBoolPrefixFullTextQueryAsync(string customerFullName)
        {
            return Ok(await _repository.MatchBoolPrefixFullTextQueryAsync(customerFullName));
        }
        [HttpGet]
        public async Task<IActionResult> MatchPhraseQueryFullTextAsync(string customerFullName)
        {
            return Ok(await _repository.MatchPhraseFullTextQueryAsync(customerFullName));
        }
        [HttpGet]
        public async Task<IActionResult> CompoundQueryExampleOneAsync(string cityName, double taxfulTotalPrice, string categoryName, string menufacturer)
        {
            return Ok(await _repository.CompoundQueryExampleOneAsync(cityName,taxfulTotalPrice,categoryName,menufacturer));
        }
    }
}
