using UnityEngine;

public class PizzaIngredientReceiver : MonoBehaviour
{
    [SerializeField] private PizzaAssembly pizza;

    private void Reset()
    {
        pizza = GetComponentInParent<PizzaAssembly>();
    }

    private void Awake()
    {
        if (pizza == null)
            pizza = GetComponentInParent<PizzaAssembly>();
    }

    private void OnTriggerEnter(Collider other)
    {
        PizzaIngredient ingredient =
            other.GetComponentInParent<PizzaIngredient>();

        if (ingredient == null)
            return;

        pizza.TryAddIngredient(ingredient);
    }
}