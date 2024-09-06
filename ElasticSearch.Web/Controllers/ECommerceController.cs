using ElasticSearch.Web.Services;
using ElasticSearch.Web.ViewMoels;
using Microsoft.AspNetCore.Mvc;

namespace ElasticSearch.Web.Controllers
{
    public class ECommerceController : Controller
    {
        private readonly ECommerceService _service;

        public ECommerceController(ECommerceService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Search([FromQuery] SeachPageViewModel searchPageView)
        {
            var (eCommerceList,totalCount,pageLinkCount)=await  _service.SearchAsync(searchPageView.SearchViewModel, searchPageView.Page, searchPageView.PageSize);
            searchPageView.List = eCommerceList;
            searchPageView.PageLinkCount = pageLinkCount;
            searchPageView.TotalCount = totalCount;
            return View(searchPageView);
        }
    }
}
