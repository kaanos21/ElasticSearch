using ElasticSearch.Web.Model;
using ElasticSearch.Web.Services;
using ElasticSearch.Web.ViewMoels;
using Microsoft.AspNetCore.Mvc;

namespace ElasticSearch.Web.Controllers
{
    public class BlogController : Controller
    {
        private readonly BlogService _blogService;

        public BlogController(BlogService blogService)
        {
            _blogService = blogService;
        }
        [HttpGet]
        public IActionResult Search()
        {
            return View(new List<Blog>());
        }
        [HttpPost]
        public async Task<IActionResult> Search(string searchText)
        {
            var blogList=await _blogService.SearchAsync(searchText);
            return View(blogList);
        }

        [HttpGet]
        public IActionResult Save()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Save(BlogCreateViewModel model)
        {
           var isSucces= await _blogService.SaveAsync(model);
            if (!isSucces)
            {
                TempData["result"] = "başarısız";
                return RedirectToAction("ZAZAZAZZAZA");
            }
            TempData["result"] = "başarılı";
            return RedirectToAction("lololololollo");
        }
    }
}
