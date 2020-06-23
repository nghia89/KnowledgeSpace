﻿using KnowledgeSpace.ViewModels.Contents;
using KnowledgeSpace.WebPortal.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace KnowledgeSpace.WebPortal.Services
{
    public class CategoryApiClient :BaseApiClient, ICategoryApiClient
    {
        public CategoryApiClient(IHttpClientFactory httpClientFactory,
              IConfiguration configuration,
              IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, configuration, httpContextAccessor)
        {
        }

        public async Task<List<CategoryVm>> GetCategories()
        {
            return await GetListAsync<CategoryVm>("/api/categories");
        }

        public async Task<CategoryVm> GetCategoryById(int id)
        {
            return await GetAsync<CategoryVm>("categories/" + id);
        }
    }
}
