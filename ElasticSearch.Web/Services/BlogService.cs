using ElasticSearch.Web.Model;
using ElasticSearch.Web.Repository;
using ElasticSearch.Web.ViewMoels;

namespace ElasticSearch.Web.Services
{
    public class BlogService
    {
        private readonly BlogRepository _repository;
        public BlogService(BlogRepository repository)
        {
            _repository = repository;
        }
        public async Task<bool> SaveAsync(BlogCreateViewModel model)
        {
            Blog newBlog = new Blog()
            {
                Title = model.Title,
                UserId = Guid.NewGuid(),
                Content = model.Content,
                Tags = model.Tags.Split(",")
            };
            var isCreated = await _repository.SaveAsync(newBlog);
            return isCreated != null;
        }

      public async Task<List<Blog>> SearchAsync(string searchText)
        {
            return await _repository.SearchAsync(searchText);
        }
    }
}
