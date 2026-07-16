using UnityEngine;

[System.Serializable]
public class PizzaRecipe
{
    public string recipeName = "Pepperoni Pizza";

    public bool cheese;
    public bool pepperoni;
    public bool jalapeno;
}

public class PizzaAssembly : MonoBehaviour
{
    [Header("Topping Visuals")]
    [SerializeField] private GameObject cheeseVisual;
    [SerializeField] private GameObject pepperoniVisual;
    [SerializeField] private GameObject jalapenoVisual;

    public bool HasCheese { get; private set; }
    public bool HasPepperoni { get; private set; }
    public bool HasJalapeno { get; private set; }

    public bool IsServed { get; private set; }

    private void Awake()
    {
        UpdateVisuals();
    }

    public bool TryAddIngredient(PizzaIngredient ingredient)
    {
        if (ingredient == null || ingredient.IsConsumed || IsServed)
            return false;

        // Refuse duplicate portions.
        switch (ingredient.IngredientType)
        {
            case PizzaIngredientType.Cheese:
                if (HasCheese)
                    return false;
                break;

            case PizzaIngredientType.Pepperoni:
                if (HasPepperoni)
                    return false;
                break;

            case PizzaIngredientType.Jalapeno:
                if (HasJalapeno)
                    return false;
                break;
        }

        if (!ingredient.TryConsume())
            return false;

        switch (ingredient.IngredientType)
        {
            case PizzaIngredientType.Cheese:
                HasCheese = true;
                break;

            case PizzaIngredientType.Pepperoni:
                HasPepperoni = true;
                break;

            case PizzaIngredientType.Jalapeno:
                HasJalapeno = true;
                break;
        }

        UpdateVisuals();

        Debug.Log($"{ingredient.IngredientType} added to {name}.");

        // Removes the loose physical ingredient.
        Destroy(ingredient.gameObject);

        return true;
    }

    public bool MatchesRecipe(PizzaRecipe recipe)
    {
        if (recipe == null)
            return false;

        return HasCheese == recipe.cheese
            && HasPepperoni == recipe.pepperoni
            && HasJalapeno == recipe.jalapeno;
    }

    public string GetIngredientDescription()
    {
        string description = "Tomato base";

        if (HasCheese)
            description += ", cheese";

        if (HasPepperoni)
            description += ", pepperoni";

        if (HasJalapeno)
            description += ", jalapeno";

        return description;
    }

    public void MarkAsServed()
    {
        IsServed = true;
    }

    private void UpdateVisuals()
    {
        if (cheeseVisual != null)
            cheeseVisual.SetActive(HasCheese);

        if (pepperoniVisual != null)
            pepperoniVisual.SetActive(HasPepperoni);

        if (jalapenoVisual != null)
            jalapenoVisual.SetActive(HasJalapeno);
    }
}