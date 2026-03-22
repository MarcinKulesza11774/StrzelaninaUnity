using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    public Light flashlight;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && flashlight != null)
            flashlight.enabled = false;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && flashlight != null)
            flashlight.enabled = true;
    }
}
