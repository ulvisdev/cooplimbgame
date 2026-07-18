using UnityEngine;

public class PizzaReceivingTrigger : MonoBehaviour
{
    [SerializeField] private CustomerOrder customerOrder;

    private void Awake()
    {
        if (customerOrder == null)
        {
            customerOrder =
                GetComponentInParent<CustomerOrder>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (customerOrder == null)
        {
            Debug.LogWarning(
                "PizzaReceivingTrigger could not find CustomerOrder."
            );

            return;
        }

        customerOrder.HandlePizzaEntered(other);
    }
}