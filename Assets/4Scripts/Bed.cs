using UnityEngine;

public class Bed : MonoBehaviour
{
    private bool canPassDay = false;

    private void Awake()
    {
        if (SceneLoadManager.Instance.prevSceneName == "Farm")
            canPassDay = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && canPassDay
            && Mathf.Abs(transform.position.x - collision.transform.position.x) <= 0.1f)
        {
            canPassDay = false;
            GameManager.Instance.dayTimeManager.NextDay();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            canPassDay = true;
    }
}
