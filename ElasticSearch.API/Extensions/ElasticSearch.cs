﻿using Elastic.Clients.Elasticsearch;

using Elastic.Transport;

using System.Runtime.CompilerServices;

namespace ElasticSearch.API.Extensions
{
    public static class ElasticSearchExt
    {
        public static void AddElastic(this IServiceCollection services,IConfiguration configuration)
        {
            var userName = configuration.GetSection("Elastic")["Username"];
            var password = configuration.GetSection("Elastic")["Password"];
            var settings = new ElasticsearchClientSettings(new Uri(configuration.GetSection("Elastic")["Url"]!)).Authentication(new BasicAuthentication(userName, password));
            
            

            var client=new ElasticsearchClient(settings);

            services.AddSingleton(client);
        }
    }
}
