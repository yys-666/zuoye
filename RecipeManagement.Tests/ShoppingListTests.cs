using System.Collections.ObjectModel;
using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// Tests for the shopping list backed by List&lt;string&gt;.
/// </summary>
public sealed class ShoppingListTests
{
    [Fact]
    public void AddIngredients_CopiesIngredientsInOrderAndReturnsCount()
    {
        var manager = new RecipeManager(new[]
        {
            MakeRecipe(1, "Soup", "carrot", "onion", "salt")
        });

        int added = manager.AddIngredientsToShoppingList(1);

        Assert.Equal(3, added);
        Assert.Equal(3, manager.ShoppingItemCount);
        Assert.Equal(new[] { "carrot", "onion", "salt" }, manager.GetShoppingList());
    }

    [Fact]
    public void AddIngredients_ReturnsZeroWhenRecipeMissing()
    {
        var manager = new RecipeManager(Array.Empty<Recipe>());
        Assert.Equal(0, manager.AddIngredientsToShoppingList(404));
        Assert.Empty(manager.GetShoppingList());
    }

    [Fact]
    public void AddIngredients_AppendsAfterExistingItems()
    {
        var manager = new RecipeManager(new[]
        {
            MakeRecipe(1, "Soup", "carrot"),
            MakeRecipe(2, "Bread", "flour")
        });

        manager.AddIngredientsToShoppingList(1);
        manager.AddIngredientsToShoppingList(2);

        Assert.Equal(new[] { "carrot", "flour" }, manager.GetShoppingList());
    }

    [Fact]
    public void GetShoppingList_IsEmptyForANewManager()
    {
        var manager = new RecipeManager(Array.Empty<Recipe>());
        Assert.Empty(manager.GetShoppingList());
    }

    [Fact]
    public void ClearShoppingList_RemovesAllItems()
    {
        var manager = new RecipeManager(new[]
        {
            MakeRecipe(1, "Soup", "carrot", "onion")
        });
        manager.AddIngredientsToShoppingList(1);

        manager.ClearShoppingList();

        Assert.Empty(manager.GetShoppingList());
        Assert.Equal(0, manager.ShoppingItemCount);
    }

    [Fact]
    public void GetShoppingList_ReturnsReadOnlyWrapper()
    {
        var manager = new RecipeManager(new[]
        {
            MakeRecipe(1, "Soup", "carrot")
        });
        manager.AddIngredientsToShoppingList(1);

        IReadOnlyList<string> items = manager.GetShoppingList();
        Assert.IsType<ReadOnlyCollection<string>>(items);
        Assert.Throws<NotSupportedException>(() =>
        {
            ((IList<string>)items).Add("mutate");
        });
    }

    private static Recipe MakeRecipe(int id, string title, params string[] ingredients) => new()
    {
        Id = id,
        Title = title,
        Ingredients = ingredients.ToList(),
        Instructions = new List<string> { "Mix" }
    };
}
