using UnityEngine;
public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    public bool HasScrewdriver = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}
