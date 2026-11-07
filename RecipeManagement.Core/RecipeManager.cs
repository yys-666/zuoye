using System;
using System.Collections.Generic;

namespace RecipeManagement.Core;

/// <summary>
/// Implements the recipe manager using the five nominated collections as
/// private fields. Day 1 builds the recipe catalogue (Dictionary) and the
/// shopping list (List); Day 2 adds the cooking plan (LinkedList) and the
/// removed-recipe history (Stack); the instruction queue arrives on Day 3.
/// </summary>
public sealed class RecipeManager : IRecipeManager
{
    private readonly Dictionary<int, Recipe> _recipes = new();
    private readonly List<string> _shoppingList = new();
    private readonly LinkedList<int> _cookingPlan = new();
    private readonly Stack<int> _removedRecipes = new();

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
    public int CookingPlanCount => _cookingPlan.Count;
    public int PendingInstructionCount => 0;
    public int RemovedRecipeCount => _removedRecipes.Count;

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
        if (!_recipes.ContainsKey(recipeId) || _cookingPlan.Contains(recipeId))
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

    public bool AddRecipeToCookingPlan(int recipeId)
    {
        if (!_recipes.ContainsKey(recipeId) || _cookingPlan.Contains(recipeId))
        {
            return false;
        }

        _cookingPlan.AddLast(recipeId);
        return true;
    }

    public bool RemoveRecipeFromCookingPlan(int recipeId)
    {
        LinkedListNode<int>? node = _cookingPlan.Find(recipeId);
        if (node is null)
        {
            return false;
        }

        _cookingPlan.Remove(node);
        _removedRecipes.Push(recipeId);
        return true;
    }

    public bool RestoreLastRemovedRecipe()
    {
        if (_removedRecipes.Count == 0)
        {
            return false;
        }

        int recipeId = _removedRecipes.Peek();
        if (!_recipes.ContainsKey(recipeId) || _cookingPlan.Contains(recipeId))
        {
            return false;
        }

        _removedRecipes.Pop();
        _cookingPlan.AddLast(recipeId);
        return true;
    }

    public int? PeekLastRemovedRecipe() =>
        _removedRecipes.Count == 0 ? null : _removedRecipes.Peek();

    public IReadOnlyList<int> GetCookingPlan() =>
        _cookingPlan.ToList().AsReadOnly();

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
