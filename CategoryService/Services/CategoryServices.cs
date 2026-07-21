using CategoryService.Data;
using CategoryService.Model;
using Microsoft.EntityFrameworkCore;

namespace CategoryService.Services
{
    public class CategoryServices : ICategoryService
    {

        private readonly CategoryDbContext _dbContext;

        public CategoryServices(CategoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Category?> CreateNewCategoryCourse(string categoryName)
        {
            if (!string.IsNullOrEmpty(categoryName))
            {
                var newCategory = new Category() { CategoryName = categoryName };
                await _dbContext.categories.AddAsync(newCategory);
                await _dbContext.SaveChangesAsync();
                return newCategory;
            }
            else
                return null;
        }

        public async Task DeleteCategory(Guid categoryId)
        {
            var category = await _dbContext.categories.FindAsync(categoryId);
            if (category != null)
            {
                _dbContext.categories.Remove(category);
               await _dbContext.SaveChangesAsync();
            }


        }

        public async Task<List<Category>> GetAllCategory()
        {
            var categoryList = await _dbContext.categories.ToListAsync();
            return categoryList.Any() ? categoryList : new List<Category>();
        }

        public async Task<Category> GetCategoryById(Guid categoryId)
        {

            var category = await _dbContext.categories.FindAsync(categoryId);
            return category != null ? category : null;

        }
    }
}
