using UnityEngine;

public class FinalDeliveryZone : MonoBehaviour
{
    private bool deliveryCompleted;

    private void OnTriggerEnter(Collider other)
    {
        if (deliveryCompleted)
            return;

        FinalDeliveryPizza finalPizza =
            other.GetComponentInParent<FinalDeliveryPizza>();

        if (finalPizza == null)
            return;

        deliveryCompleted = true;

        if (PizzaShiftManager.Instance != null)
            PizzaShiftManager.Instance.CompleteFinalDelivery();

        Destroy(finalPizza.gameObject);
    }
}