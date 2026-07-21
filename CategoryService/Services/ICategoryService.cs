using CategoryService.Model;

namespace CategoryService.Services
{
    public interface ICategoryService
    {
        Task<Category?> CreateNewCategoryCourse(string categoryName);
        Task<Category> GetCategoryById(Guid categoryId);
        Task<List<Category>> GetAllCategory();
        Task DeleteCategory(Guid categoryId);
    }
}
