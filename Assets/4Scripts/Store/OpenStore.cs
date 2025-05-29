using UnityEngine;

public class OpenStore : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.uiManager.ToggleStore();
        }
    }
}