using KnowledgeSpace.ViewModels;
using KnowledgeSpace.ViewModels.Contents;
using KnowledgeSpace.WebPortal.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace KnowledgeSpace.WebPortal.Services
{
    public class KnowledgeBaseApiClient : BaseApiClient, IKnowledgeBaseApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public KnowledgeBaseApiClient(IHttpClientFactory httpClientFactory,
              IConfiguration configuration,
              IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, configuration, httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Pagination<KnowledgeBaseQuickVm>> GetKnowledgeBasesByCategoryId(int categoryId, int pageIndex, int pageSize)
        {
            var apiUrl = $"knowledgeBases/filter?categoryId={categoryId}&pageIndex={pageIndex}&pageSize={pageSize}";
            return await GetAsync<Pagination<KnowledgeBaseQuickVm>>(apiUrl);
        }

        public async Task<List<KnowledgeBaseQuickVm>> GetLatestKnowledgeBases(int take)
        {
            return await GetListAsync<KnowledgeBaseQuickVm>($"knowledgeBases/latest/{take}");
        }

        public async Task<List<KnowledgeBaseQuickVm>> GetPopularKnowledgeBases(int take)
        {         
            return await GetListAsync<KnowledgeBaseQuickVm>($"knowledgeBases/popular/{take}");
        }

        public async Task<List<LabelVm>> GetPopularLabels(int take)
        {
            return await GetListAsync<LabelVm>($"labels/popular/{take}");
        }

        public async Task<List<LabelVm>> GetLabelsByKnowledgeBaseId(int id)
        {
            return await GetListAsync<LabelVm>($"knowledgeBases/{id}/labels");
        }
        public async Task<KnowledgeBaseVm> GetKnowledgeBaseDetail(int id)
        {
            return await GetAsync<KnowledgeBaseVm>($"knowledgeBases/{id}");
        }

        public async Task<Pagination<KnowledgeBaseQuickVm>> SearchKnowledgeBase(string keyword, int pageIndex, int pageSize)
        {
            var apiUrl = $"knowledgeBases/filter?filter={keyword}&pageIndex={pageIndex}&pageSize={pageSize}";      
            return await GetAsync<Pagination<KnowledgeBaseQuickVm>>(apiUrl);
        }
        public async Task<Pagination<KnowledgeBaseQuickVm>> GetKnowledgeBasesByTagId(string tagId, int pageIndex, int pageSize)
        {
            var apiUrl = $"knowledgeBases/tags/{tagId}?pageIndex={pageIndex}&pageSize={pageSize}";          
            return await GetAsync<Pagination<KnowledgeBaseQuickVm>>(apiUrl);
        }

        public async Task<List<CommentVm>> GetRecentComments(int take)
        {
            return await GetListAsync<CommentVm>($"knowledgeBases/comments/recent/{take}");
        }

        public async Task<CommentVm> PostComment(CommentCreateRequest request)
        {
           var data= await PostAsync<CommentCreateRequest, CommentVm>($"knowledgeBases/{request.KnowledgeBaseId}/comments", request);
            return data;
        }

        public async Task<Pagination<CommentVm>> GetCommentsTree(int knowledgeBaseId, int pageIndex, int pageSize)
        {
            return await GetAsync<Pagination<CommentVm>>($"knowledgeBases/{knowledgeBaseId}/comments/tree?pageIndex={pageIndex}&pageSize={pageSize}");
        }

        public async Task<Pagination<CommentVm>> GetRepliedComments(int knowledgeBaseId, int rootCommentId, int pageIndex, int pageSize)
        {
            return await GetAsync<Pagination<CommentVm>>($"knowledgeBases/{knowledgeBaseId}/comments/{rootCommentId}/replied?pageIndex={pageIndex}&pageSize={pageSize}");
        }

        public async Task<bool> PostKnowlegdeBase(KnowledgeBaseCreateRequest request)
        {
            var client = _httpClientFactory.CreateClient("BackendApi");

            client.BaseAddress = new Uri(_configuration["BackendApiUrl"]);
            using var requestContent = new MultipartFormDataContent();

            if (request.Attachments?.Count > 0)
            {
                foreach (var item in request.Attachments)
                {
                    byte[] data;
                    using (var br = new BinaryReader(item.OpenReadStream()))
                    {
                        data = br.ReadBytes((int)item.OpenReadStream().Length);
                    }
                    ByteArrayContent bytes = new ByteArrayContent(data);
                    requestContent.Add(bytes, "attachments", item.FileName);
                }
            }
            requestContent.Add(new StringContent(request.CategoryId.ToString()), "categoryId");
            requestContent.Add(new StringContent(request.Title.ToString()), "title");
            requestContent.Add(new StringContent(request.Problem.ToString()), "problem");
            requestContent.Add(new StringContent(request.Note.ToString()), "note");
            requestContent.Add(new StringContent(request.Description.ToString()), "description");
            requestContent.Add(new StringContent(request.Environment.ToString()), "environment");
            requestContent.Add(new StringContent(request.StepToReproduce.ToString()), "stepToReproduce");
            requestContent.Add(new StringContent(request.ErrorMessage.ToString()), "errorMessage");
            requestContent.Add(new StringContent(request.Workaround.ToString()), "workaround");
            if (request.Labels?.Length > 0)
            {
                foreach (var label in request.Labels)
                {
                    requestContent.Add(new StringContent(label.ToString()), "labels");
                }
            }

            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsync($"/api/knowledgeBases/", requestContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PutKnowlegdeBase(int id, KnowledgeBaseCreateRequest request)
        {
            var client = _httpClientFactory.CreateClient("BackendApi");

            client.BaseAddress = new Uri(_configuration["BackendApiUrl"]);
            using var requestContent = new MultipartFormDataContent();

            if (request.Attachments?.Count > 0)
            {
                foreach (var item in request.Attachments)
                {
                    byte[] data;
                    using (var br = new BinaryReader(item.OpenReadStream()))
                    {
                        data = br.ReadBytes((int)item.OpenReadStream().Length);
                    }
                    ByteArrayContent bytes = new ByteArrayContent(data);
                    requestContent.Add(bytes, "attachments", item.FileName);
                }
            }
            requestContent.Add(new StringContent(request.CategoryId.ToString()), "categoryId");
            requestContent.Add(new StringContent(request.Title.ToString()), "title");
            requestContent.Add(new StringContent(request.Problem.ToString()), "problem");
            requestContent.Add(new StringContent(request.Note.ToString()), "note");
            requestContent.Add(new StringContent(request.Description.ToString()), "description");
            requestContent.Add(new StringContent(request.Environment.ToString()), "environment");
            requestContent.Add(new StringContent(request.StepToReproduce.ToString()), "stepToReproduce");
            requestContent.Add(new StringContent(request.ErrorMessage.ToString()), "errorMessage");
            requestContent.Add(new StringContent(request.Workaround.ToString()), "workaround");
            if (request.Labels?.Length > 0)
            {
                foreach (var label in request.Labels)
                {
                    requestContent.Add(new StringContent(label), "labels");
                }
            }

            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PutAsync($"/api/knowledgeBases/{id}", requestContent);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> UpdateViewCount(int id)
        {
            return await PutAsync<object, bool>($"knowledgeBases/{id}/view-count", null, false);
        }
        public async Task<int> PostVote(VoteCreateRequest request)
        {
            return await PostAsync<VoteCreateRequest, int>($"knowledgeBases/{request.KnowledgeBaseId}/votes", null);
        }

        public async Task<ReportVm> PostReport(ReportCreateRequest request)
        {
            return await PostAsync<ReportCreateRequest, ReportVm>($"knowledgeBases/{request.KnowledgeBaseId}/reports", request);
        }
    }
}
