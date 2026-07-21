using CategoryService.Services;

namespace CategoryService.EndPoints
{
    public static class CategoryEndPoints
    {


        public static void MapCategoryEndPoints(this WebApplication app)
        {

            var group = app.MapGroup("/api/categories/");

            group.MapPost("", async (string categoryName, ICategoryService categoryService) =>
            {
                var newCategoryCourse = await categoryService.CreateNewCategoryCourse(categoryName);
                return newCategoryCourse != null ? Results.Ok(newCategoryCourse) : Results.BadRequest();

            });
            group.MapGet("/{categoryId}", async (Guid categoryId, ICategoryService categoryService) =>
            {
                var category = await categoryService.GetCategoryById(categoryId);
                return category != null ? Results.Ok(category) : Results.NotFound($"category with ID {categoryId} not found");

            });

            group.MapGet("", async (ICategoryService categoryService) =>
            {
                var categoryList = await categoryService.GetAllCategory();
                return Results.Ok(categoryList);

            });


            group.MapDelete("/{categoryId}", async (Guid categoryId, ICategoryService categoryService) =>
            {
                await categoryService.DeleteCategory(categoryId);
                return Results.NoContent();

            });





        }


    }
}
