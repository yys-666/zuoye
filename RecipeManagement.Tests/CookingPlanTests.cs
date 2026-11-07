using System.Collections.ObjectModel;
using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// Tests for the cooking plan backed by LinkedList&lt;int&gt;.
/// </summary>
public sealed class CookingPlanTests
{
    [Fact]
    public void AddToPlan_AppendsRecipeAndReturnsTrue()
    {
        var manager = ManagerWithRecipes(1, 2, 3);

        Assert.True(manager.AddRecipeToCookingPlan(1));
        Assert.True(manager.AddRecipeToCookingPlan(2));
        Assert.Equal(2, manager.CookingPlanCount);
        Assert.Equal(new[] { 1, 2 }, manager.GetCookingPlan());
    }

    [Fact]
    public void AddToPlan_ReturnsFalseWhenRecipeMissing()
    {
        var manager = ManagerWithRecipes(1);
        Assert.False(manager.AddRecipeToCookingPlan(404));
        Assert.Equal(0, manager.CookingPlanCount);
    }

    [Fact]
    public void AddToPlan_ReturnsFalseWhenRecipeAlreadyPlanned()
    {
        var manager = ManagerWithRecipes(1);
        Assert.True(manager.AddRecipeToCookingPlan(1));
        Assert.False(manager.AddRecipeToCookingPlan(1));
        Assert.Single(manager.GetCookingPlan());
    }

    [Fact]
    public void RemoveFromPlan_RemovesRecipeAndReturnsTrue()
    {
        var manager = ManagerWithRecipes(1, 2, 3);
        manager.AddRecipeToCookingPlan(1);
        manager.AddRecipeToCookingPlan(2);
        manager.AddRecipeToCookingPlan(3);

        Assert.True(manager.RemoveRecipeFromCookingPlan(2));
        Assert.Equal(new[] { 1, 3 }, manager.GetCookingPlan());
        Assert.Equal(2, manager.CookingPlanCount);
    }

    [Fact]
    public void RemoveFromPlan_ReturnsFalseWhenRecipeNotPlanned()
    {
        var manager = ManagerWithRecipes(1);
        Assert.False(manager.RemoveRecipeFromCookingPlan(1));
    }

    [Fact]
    public void RemoveFromPlan_ReturnsFalseForMissingRecipe()
    {
        var manager = ManagerWithRecipes(1);
        Assert.False(manager.RemoveRecipeFromCookingPlan(999));
    }

    [Fact]
    public void GetCookingPlan_ReturnsReadOnlyWrapperInLinkedListOrder()
    {
        var manager = ManagerWithRecipes(1, 2);
        manager.AddRecipeToCookingPlan(2);
        manager.AddRecipeToCookingPlan(1);

        IReadOnlyList<int> plan = manager.GetCookingPlan();
        Assert.IsType<ReadOnlyCollection<int>>(plan);
        Assert.Equal(new[] { 2, 1 }, plan);
        Assert.Throws<NotSupportedException>(() => ((IList<int>)plan).Add(9));
    }

    [Fact]
    public void RemoveRecipe_IsBlockedWhileRecipeRemainsInPlan()
    {
        var manager = ManagerWithRecipes(1);
        manager.AddRecipeToCookingPlan(1);

        Assert.False(manager.RemoveRecipe(1));
        Assert.NotNull(manager.FindRecipe(1));
    }

    [Fact]
    public void RemoveRecipe_SucceedsAfterRecipeLeavesPlan()
    {
        var manager = ManagerWithRecipes(1);
        manager.AddRecipeToCookingPlan(1);
        manager.RemoveRecipeFromCookingPlan(1);

        Assert.True(manager.RemoveRecipe(1));
        Assert.Null(manager.FindRecipe(1));
    }

    private static RecipeManager ManagerWithRecipes(params int[] ids)
    {
        var recipes = ids.Select(id => new Recipe
        {
            Id = id,
            Title = $"Recipe {id}",
            Ingredients = new List<string> { "salt" },
            Instructions = new List<string> { "Step" }
        });
        return new RecipeManager(recipes);
    }
}
