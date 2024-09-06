using System.Drawing.Printing;

namespace ElasticSearch.Web.ViewMoels
{
    public class SeachPageViewModel
    {
        public long TotalCount { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public long PageLinkCount {  get; set; }
        public List<ECommerceViewModel> List {  get; set; }
        public ECommerceSeachViewModel SearchViewModel {  get; set; }
        public string CreatePageUrl(HttpRequest request,int page,int pageSize)
        {
            var CurrentUrl = new Uri($"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}").AbsoluteUri;
            if (CurrentUrl.Contains("page", StringComparison.OrdinalIgnoreCase))
            {
                CurrentUrl=CurrentUrl.Replace($"Page={Page}",$"Page={page}",StringComparison.OrdinalIgnoreCase);
                CurrentUrl = CurrentUrl.Replace($"PageSize={pageSize}", $"Page ={ pageSize}",StringComparison.OrdinalIgnoreCase) ;
            }
            else
            {
                CurrentUrl=$"{CurrentUrl}?Page={page}";
                CurrentUrl=$"{CurrentUrl}?PageSize={pageSize}";
            }
            return CurrentUrl;
        }
    }
}
