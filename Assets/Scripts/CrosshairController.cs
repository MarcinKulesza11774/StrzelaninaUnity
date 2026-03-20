using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    public PlayerMovement playerMovement;

    void Update()
    {
        Debug.Log("IsAiming: " + playerMovement.IsAiming);
        gameObject.SetActive(playerMovement.IsAiming);
    }
}