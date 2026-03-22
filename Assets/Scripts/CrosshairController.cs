using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    public PlayerMovement playerMovement;

    void Update()
    {
        gameObject.SetActive(playerMovement.IsAiming);
    }
}