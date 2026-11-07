using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// Tests for the removed-recipe history backed by Stack&lt;int&gt;.
/// </summary>
public sealed class RemovedHistoryTests
{
    [Fact]
    public void PeekLastRemoved_ReturnsNullWhenHistoryIsEmpty()
    {
        var manager = ManagerWithRecipes(1);
        Assert.Null(manager.PeekLastRemovedRecipe());
        Assert.Equal(0, manager.RemovedRecipeCount);
    }

    [Fact]
    public void RemoveFromPlan_PushesIdIntoHistory()
    {
        var manager = ManagerWithRecipes(1);
        manager.AddRecipeToCookingPlan(1);
        manager.RemoveRecipeFromCookingPlan(1);

        Assert.Equal(1, manager.RemovedRecipeCount);
        Assert.Equal(1, manager.PeekLastRemovedRecipe());
    }

    [Fact]
    public void Peek_DoesNotPopTheHistory()
    {
        var manager = ManagerWithRecipes(1);
        manager.AddRecipeToCookingPlan(1);
        manager.RemoveRecipeFromCookingPlan(1);

        Assert.Equal(1, manager.PeekLastRemovedRecipe());
        Assert.Equal(1, manager.PeekLastRemovedRecipe());
        Assert.Equal(1, manager.RemovedRecipeCount);
    }

    [Fact]
    public void History_FollowsLifoOrderForMultipleRemovals()
    {
        var manager = ManagerWithRecipes(1, 2, 3);
        foreach (int id in new[] { 1, 2, 3 })
        {
            manager.AddRecipeToCookingPlan(id);
        }

        manager.RemoveRecipeFromCookingPlan(1);
        manager.RemoveRecipeFromCookingPlan(2);
        manager.RemoveRecipeFromCookingPlan(3);

        Assert.Equal(3, manager.RemovedRecipeCount);
        Assert.Equal(3, manager.PeekLastRemovedRecipe());
    }

    [Fact]
    public void Restore_MovesLastRemovedRecipeBackToPlan()
    {
        var manager = ManagerWithRecipes(1, 2);
        manager.AddRecipeToCookingPlan(1);
        manager.AddRecipeToCookingPlan(2);
        manager.RemoveRecipeFromCookingPlan(2);

        Assert.True(manager.RestoreLastRemovedRecipe());
        Assert.Equal(new[] { 1, 2 }, manager.GetCookingPlan());
        Assert.Equal(0, manager.RemovedRecipeCount);
    }

    [Fact]
    public void Restore_ReturnsFalseWhenHistoryIsEmpty()
    {
        var manager = ManagerWithRecipes(1);
        Assert.False(manager.RestoreLastRemovedRecipe());
    }

    [Fact]
    public void Restore_ReturnsFalseAndKeepsStackWhenRecipeWasDeleted()
    {
        var manager = ManagerWithRecipes(1);
        manager.AddRecipeToCookingPlan(1);
        manager.RemoveRecipeFromCookingPlan(1);
        Assert.True(manager.RemoveRecipe(1));

        Assert.False(manager.RestoreLastRemovedRecipe());
        Assert.Equal(1, manager.RemovedRecipeCount);
        Assert.Equal(1, manager.PeekLastRemovedRecipe());
    }

    [Fact]
    public void Restore_ReturnsFalseAndKeepsStackWhenRecipeAlreadyInPlan()
    {
        var manager = ManagerWithRecipes(1);
        manager.AddRecipeToCookingPlan(1);
        manager.RemoveRecipeFromCookingPlan(1);
        manager.AddRecipeToCookingPlan(1);

        Assert.False(manager.RestoreLastRemovedRecipe());
        Assert.Equal(1, manager.RemovedRecipeCount);
        Assert.Single(manager.GetCookingPlan());
    }

    [Fact]
    public void FailedRemoveFromPlan_DoesNotChangeHistory()
    {
        var manager = ManagerWithRecipes(1);
        Assert.False(manager.RemoveRecipeFromCookingPlan(1));
        Assert.Equal(0, manager.RemovedRecipeCount);
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
