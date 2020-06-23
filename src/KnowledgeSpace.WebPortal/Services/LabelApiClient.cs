using KnowledgeSpace.ViewModels.Contents;
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
    public class LabelApiClient:BaseApiClient, ILabelApiClient
    {

        public LabelApiClient(IHttpClientFactory httpClientFactory,
              IConfiguration configuration,
              IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, configuration, httpContextAccessor)
        {
        }

        public async Task<LabelVm> GetLabelById(string labelId)
        {
            return await GetAsync<LabelVm>($"labels/{labelId}");
        }

        public async Task<List<LabelVm>> GetPopularLabels(int take)
        {
            return await GetListAsync<LabelVm>($"labels/popular/{take}");
        }
    }
}
