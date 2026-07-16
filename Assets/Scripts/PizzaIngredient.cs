using UnityEngine;

public enum PizzaIngredientType
{
    Cheese,
    Pepperoni,
    Jalapeno
}

public class PizzaIngredient : MonoBehaviour
{
    [SerializeField]
    private PizzaIngredientType ingredientType;

    public PizzaIngredientType IngredientType => ingredientType;
    public bool IsConsumed { get; private set; }

    public bool TryConsume()
    {
        if (IsConsumed)
            return false;

        IsConsumed = true;

        // Prevent the same ingredient triggering multiple times
        // before Unity destroys it.
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider ingredientCollider in colliders)
            ingredientCollider.enabled = false;

        return true;
    }
}