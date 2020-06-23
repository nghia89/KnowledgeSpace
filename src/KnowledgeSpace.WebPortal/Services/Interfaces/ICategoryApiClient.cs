using KnowledgeSpace.ViewModels.Contents;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KnowledgeSpace.WebPortal.Services.Interfaces
{
    public interface ICategoryApiClient
    {
        Task<List<CategoryVm>> GetCategories();
        Task<CategoryVm> GetCategoryById(int id);
    }
}
