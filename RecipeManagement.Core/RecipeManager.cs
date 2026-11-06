using System;
using System.Collections.Generic;

namespace RecipeManagement.Core;

/// <summary>
/// Implements the recipe manager using the five nominated collections as
/// private fields. Day 1 builds the recipe catalogue (Dictionary) and the
/// shopping list (List); the remaining collections are added on later days.
/// </summary>
public sealed class RecipeManager : IRecipeManager
{
    private readonly Dictionary<int, Recipe> _recipes = new();
    private readonly List<string> _shoppingList = new();

    public RecipeManager(IEnumerable<Recipe> recipes)
    {
        if (recipes is null)
        {
            throw new ArgumentNullException(nameof(recipes));
        }

        foreach (Recipe? recipe in recipes)
        {
            if (recipe is null)
            {
                throw new ArgumentNullException(
                    nameof(recipes),
                    "A recipe in the supplied collection must not be null.");
            }

            if (recipe.Id <= 0)
            {
                throw new ArgumentException(
                    $"Recipe ID must be positive. Encountered ID: {recipe.Id}.",
                    nameof(recipes));
            }

            if (string.IsNullOrWhiteSpace(recipe.Title))
            {
                throw new ArgumentException(
                    $"Recipe with ID {recipe.Id} has a blank title.",
                    nameof(recipes));
            }

            if (!_recipes.TryAdd(recipe.Id, recipe))
            {
                throw new ArgumentException(
                    $"Duplicate recipe ID: {recipe.Id}.",
                    nameof(recipes));
            }
        }
    }

    public int RecipeCount => _recipes.Count;
    public int ShoppingItemCount => _shoppingList.Count;
    public int CookingPlanCount => 0;
    public int PendingInstructionCount => 0;
    public int RemovedRecipeCount => 0;

    public bool AddRecipe(Recipe recipe)
    {
        if (recipe is null)
        {
            throw new ArgumentNullException(nameof(recipe));
        }

        if (recipe.Id <= 0 ||
            string.IsNullOrWhiteSpace(recipe.Title) ||
            _recipes.ContainsKey(recipe.Id))
        {
            return false;
        }

        _recipes.Add(recipe.Id, recipe);
        return true;
    }

    public Recipe? FindRecipe(int recipeId) =>
        _recipes.TryGetValue(recipeId, out Recipe? recipe) ? recipe : null;

    public bool RemoveRecipe(int recipeId)
    {
        if (!_recipes.ContainsKey(recipeId))
        {
            return false;
        }

        return _recipes.Remove(recipeId);
    }

    public int AddIngredientsToShoppingList(int recipeId)
    {
        if (!_recipes.TryGetValue(recipeId, out Recipe? recipe))
        {
            return 0;
        }

        _shoppingList.AddRange(recipe.Ingredients);
        return recipe.Ingredients.Count;
    }

    public IReadOnlyList<string> GetShoppingList() =>
        _shoppingList.AsReadOnly();

    public void ClearShoppingList() => _shoppingList.Clear();

    public bool AddRecipeToCookingPlan(int recipeId) =>
        throw new NotImplementedException("Day 2: implement AddRecipeToCookingPlan.");

    public bool RemoveRecipeFromCookingPlan(int recipeId) =>
        throw new NotImplementedException("Day 2: implement RemoveRecipeFromCookingPlan.");

    public bool RestoreLastRemovedRecipe() =>
        throw new NotImplementedException("Day 2: implement RestoreLastRemovedRecipe.");

    public int? PeekLastRemovedRecipe() =>
        throw new NotImplementedException("Day 2: implement PeekLastRemovedRecipe.");

    public IReadOnlyList<int> GetCookingPlan() =>
        throw new NotImplementedException("Day 2: implement GetCookingPlan.");

    public bool StartCooking(int recipeId) =>
        throw new NotImplementedException("Day 3: implement StartCooking.");

    public string? PeekNextInstruction() =>
        throw new NotImplementedException("Day 3: implement PeekNextInstruction.");

    public string? CompleteNextInstruction() =>
        throw new NotImplementedException("Day 3: implement CompleteNextInstruction.");

    public IReadOnlyList<Recipe> SearchByTitle(string searchText) =>
        throw new NotImplementedException("Part B: implement SearchByTitle.");

    public IReadOnlyList<Recipe> SearchByIngredient(string searchText) =>
        throw new NotImplementedException("Part B: implement SearchByIngredient.");

    public IReadOnlyList<Recipe> GetHighestProteinRecipes(int count) =>
        throw new NotImplementedException("Part B: implement GetHighestProteinRecipes.");

    public bool AddSavedRecipe(int recipeId) =>
        throw new NotImplementedException("Part B: implement AddSavedRecipe.");

    public bool RemoveSavedRecipe(int recipeId) =>
        throw new NotImplementedException("Part B: implement RemoveSavedRecipe.");

    public bool IsRecipeSaved(int recipeId) =>
        throw new NotImplementedException("Part B: implement IsRecipeSaved.");

    public IReadOnlyList<int> GetSavedRecipes() =>
        throw new NotImplementedException("Part B: implement GetSavedRecipes.");
}
