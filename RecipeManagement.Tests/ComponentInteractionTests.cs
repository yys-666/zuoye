using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// End-to-end tests exercising several nominated collections together.
/// </summary>
public sealed class ComponentInteractionTests
{
    [Fact]
    public void PlanRemoveHistoryRestore_WorksAcrossLinkedListAndStack()
    {
        var manager = ManagerWithRecipes(1, 2);

        Assert.True(manager.AddRecipeToCookingPlan(1));
        Assert.True(manager.AddRecipeToCookingPlan(2));
        Assert.True(manager.RemoveRecipeFromCookingPlan(2));
        Assert.Equal(2, manager.PeekLastRemovedRecipe());

        Assert.True(manager.RestoreLastRemovedRecipe());
        Assert.Equal(new[] { 1, 2 }, manager.GetCookingPlan());
        Assert.Equal(0, manager.RemovedRecipeCount);
    }

    [Fact]
    public void StartCooking_QueuesInstructionsInFileOrder()
    {
        var manager = ManagerWithRecipes(1);
        var expected = new[] { "Prep", "Cook", "Serve" };

        Assert.True(manager.StartCooking(1));
        Assert.Equal(3, manager.PendingInstructionCount);

        foreach (string step in expected)
        {
            Assert.Equal(step, manager.PeekNextInstruction());
            Assert.Equal(step, manager.CompleteNextInstruction());
        }
    }

    [Fact]
    public void Peek_MatchesTheNextDequeuedInstruction()
    {
        var manager = ManagerWithRecipes(1);
        manager.StartCooking(1);

        string? peeked = manager.PeekNextInstruction();
        string? completed = manager.CompleteNextInstruction();
        Assert.Equal(peeked, completed);
    }

    [Fact]
    public void CompletingEveryInstruction_DrainsTheQueueToZero()
    {
        var manager = ManagerWithRecipes(1);
        manager.StartCooking(1);

        while (manager.PendingInstructionCount > 0)
        {
            Assert.NotNull(manager.CompleteNextInstruction());
        }

        Assert.Equal(0, manager.PendingInstructionCount);
        Assert.Null(manager.PeekNextInstruction());
        Assert.Null(manager.CompleteNextInstruction());
    }

    [Fact]
    public void StartCooking_ReturnsFalseForMissingRecipeAndKeepsQueue()
    {
        var manager = ManagerWithRecipes(1);
        manager.StartCooking(1);

        Assert.False(manager.StartCooking(404));
        Assert.Equal(3, manager.PendingInstructionCount);
    }

    [Fact]
    public void StartCooking_ReplacesPreviousQueueOnSuccess()
    {
        var manager = ManagerWithRecipes(1, 2);
        manager.StartCooking(1);
        Assert.True(manager.StartCooking(2));

        Assert.Equal(2, manager.PendingInstructionCount);
        Assert.Equal("Boil", manager.PeekNextInstruction());
    }

    [Fact]
    public void RemoveRecipe_StaysBlockedWhileRecipeIsPlanned()
    {
        var manager = ManagerWithRecipes(1);
        manager.AddRecipeToCookingPlan(1);

        Assert.False(manager.RemoveRecipe(1));
        Assert.NotNull(manager.FindRecipe(1));
    }

    [Fact]
    public void CookingAndPlan_DoNotAffectShoppingListOrCatalogue()
    {
        var manager = ManagerWithRecipes(1, 2);
        manager.AddIngredientsToShoppingList(1);
        manager.AddRecipeToCookingPlan(2);
        manager.StartCooking(2);

        Assert.Equal(new[] { "salt", "pepper" }, manager.GetShoppingList());
        Assert.Equal(2, manager.RecipeCount);
        Assert.NotNull(manager.FindRecipe(1));
    }

    private static RecipeManager ManagerWithRecipes(params int[] ids)
    {
        var recipes = new List<Recipe>();
        foreach (int id in ids)
        {
            recipes.Add(new Recipe
            {
                Id = id,
                Title = $"Recipe {id}",
                Ingredients = new List<string> { "salt", "pepper" },
                Instructions = id == 1
                    ? new List<string> { "Prep", "Cook", "Serve" }
                    : new List<string> { "Boil", "Plate" }
            });
        }

        return new RecipeManager(recipes);
    }
}
