using Elastic.Clients.Elasticsearch;
using ElasticSearch.API.DTOs;
using ElasticSearch.API.Models;
using ElasticSearch.API.Repositories;

using System.Collections.Immutable;

using System.Net;

namespace ElasticSearch.API.Services
{
    public class ProductService
    {
        private readonly ProductRepository _productRepository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(ProductRepository productRepository, ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<ResponseDto<ProductDto>> SaveAsync(ProductCreateDto requset)
        {
            var responseProduct = await _productRepository.SaveAsync(requset.CreateProduct());
            if(responseProduct == null)
            {
                return ResponseDto<ProductDto>.Fail(new List<string> { "kayıtta hata"} , System.Net.HttpStatusCode.InternalServerError);
            }

            return ResponseDto<ProductDto>.Succes(responseProduct.CreateDto(), HttpStatusCode.Created);


        }

        public  async Task<ResponseDto<List<ProductDto>>> GetAllAsync()
        {
            var products= await _productRepository.GetAllAsync();
            var productListDto = new List<ProductDto>();
            //var productListDto= products.Select(x => new ProductDto(x.Id, x.Name, x.Price,x.Stock, new ProductFeatureDto(x.Feature.Width,x.Feature!.Height,x.Feature!.Color))).ToList();
            foreach(var x in products)
            {
                if(x.Feature is null)
                {
                    productListDto.Add(new ProductDto(x.Id,x.Name,x.Price,x.Stock,null));
                }
                else
                {
                    productListDto.Add(new ProductDto(x.Id, x.Name, x.Price, x.Stock, new ProductFeatureDto(x.Feature.Width, x.Feature!.Height, x.Feature!.Color.ToString())));
                }
            }
            return ResponseDto<List<ProductDto>>.Succes(productListDto, HttpStatusCode.OK);
        }
        public async Task<ResponseDto<ProductDto>> GetByIdAsync(string id)
        {
            var hasProduct=await _productRepository.GetByIdAsync(id);

            if (hasProduct == null)
            {
                return ResponseDto<ProductDto>.Fail("Ürün Bulunamadı",HttpStatusCode.NotFound);
            }
            var productDto=hasProduct.CreateDto();
            return ResponseDto<ProductDto>.Succes(productDto,HttpStatusCode.OK);
        }
        public async Task<ResponseDto<bool>> UpdateAsync(ProductUpdateDto updateProduct)
        {
            var isSucces=await _productRepository.UpdateAsync(updateProduct);
            if (!isSucces)
            {
                
                return ResponseDto<bool>.Fail(new List<string> { "kayıtta hata" }, System.Net.HttpStatusCode.InternalServerError);
            }
            return ResponseDto<bool>.Succes(true,HttpStatusCode.NoContent );
        }

        public async Task<ResponseDto<bool>> DeleteAsync(string id)
        {
            var deleteResponse = await _productRepository.DeleteAsync(id);

            if(deleteResponse.IsSuccess() && deleteResponse.Result == Result.NotFound)
            {
                return ResponseDto<bool>.Fail(new List<string> { "bulunamadı" }, System.Net.HttpStatusCode.NotFound);
            }
            if (!deleteResponse.IsSuccess())
            {
                deleteResponse.TryGetOriginalException(out Exception? exception);
                _logger.LogError(exception,deleteResponse.ElasticsearchServerError.Error.ToString());
                return ResponseDto<bool>.Fail(new List<string> { "kayıtta hata" }, System.Net.HttpStatusCode.InternalServerError);
            } 
            return ResponseDto<bool>.Succes(true, HttpStatusCode.NoContent);
        }
    }
}
