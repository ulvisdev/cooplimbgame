using UnityEngine;

public class LimbController : MonoBehaviour
{
    public Rigidbody rb;
    public float torquePower = 50f;

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Q))
            rb.AddTorque(Vector3.forward * torquePower);

        if (Input.GetKey(KeyCode.E))
            rb.AddTorque(Vector3.back * torquePower);
    }
}