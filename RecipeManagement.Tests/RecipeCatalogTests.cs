using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// Tests for the recipe catalogue backed by Dictionary&lt;int, Recipe&gt;.
/// </summary>
public sealed class RecipeCatalogTests
{
    [Fact]
    public void AddRecipe_StoresRecipeAndReturnsTrue()
    {
        var manager = new RecipeManager(Array.Empty<Recipe>());
        var recipe = MakeRecipe(1, "Soup");

        Assert.True(manager.AddRecipe(recipe));
        Assert.Equal(1, manager.RecipeCount);
        Assert.Same(recipe, manager.FindRecipe(1));
    }

    [Fact]
    public void AddRecipe_ThrowsWhenRecipeIsNull()
    {
        var manager = new RecipeManager(Array.Empty<Recipe>());
        Assert.Throws<ArgumentNullException>(() => manager.AddRecipe(null!));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void AddRecipe_ReturnsFalseForNonPositiveId(int id)
    {
        var manager = new RecipeManager(Array.Empty<Recipe>());
        Assert.False(manager.AddRecipe(MakeRecipe(id, "Invalid")));
        Assert.Equal(0, manager.RecipeCount);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void AddRecipe_ReturnsFalseForBlankTitle(string title)
    {
        var manager = new RecipeManager(Array.Empty<Recipe>());
        Assert.False(manager.AddRecipe(MakeRecipe(1, title)));
    }

    [Fact]
    public void AddRecipe_ReturnsFalseForDuplicateId()
    {
        var manager = new RecipeManager(new[] { MakeRecipe(1, "First") });
        Assert.False(manager.AddRecipe(MakeRecipe(1, "Second")));
        Assert.Equal(1, manager.RecipeCount);
    }

    [Fact]
    public void FindRecipe_ReturnsNullWhenIdIsMissing()
    {
        var manager = new RecipeManager(new[] { MakeRecipe(1, "Soup") });
        Assert.Null(manager.FindRecipe(999));
    }

    [Fact]
    public void RemoveRecipe_RemovesExistingRecipeAndReturnsTrue()
    {
        var manager = new RecipeManager(new[] { MakeRecipe(1, "Soup") });
        Assert.True(manager.RemoveRecipe(1));
        Assert.Null(manager.FindRecipe(1));
        Assert.Equal(0, manager.RecipeCount);
    }

    [Fact]
    public void RemoveRecipe_ReturnsFalseForMissingId()
    {
        var manager = new RecipeManager(Array.Empty<Recipe>());
        Assert.False(manager.RemoveRecipe(404));
    }

    [Fact]
    public void Constructor_ThrowsWhenAnyRecipeIsNull()
    {
        Assert.Throws<ArgumentNullException>(
            () => new RecipeManager(new Recipe?[] { null }));
    }

    [Fact]
    public void Constructor_ThrowsForNonPositiveId()
    {
        Assert.Throws<ArgumentException>(
            () => new RecipeManager(new[] { MakeRecipe(0, "Bad Id") }));
    }

    [Fact]
    public void Constructor_ThrowsForBlankTitle()
    {
        Assert.Throws<ArgumentException>(
            () => new RecipeManager(new[] { MakeRecipe(2, " ") }));
    }

    [Fact]
    public void Constructor_ThrowsForDuplicateId()
    {
        Assert.Throws<ArgumentException>(
            () => new RecipeManager(new[] { MakeRecipe(3, "A"), MakeRecipe(3, "B") }));
    }

    private static Recipe MakeRecipe(int id, string title) => new()
    {
        Id = id,
        Title = title,
        Ingredients = new List<string> { "salt", "water" },
        Instructions = new List<string> { "Mix", "Cook" }
    };
}
