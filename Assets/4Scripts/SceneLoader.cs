using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string sceneName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SceneLoadManager.Instance.StartLoadScene(sceneName, false);
        }
    }
}
