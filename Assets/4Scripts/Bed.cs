using UnityEngine;

public class Bed : MonoBehaviour
{
    public bool isPassDay = false;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isPassDay
            && Mathf.Abs(transform.position.x - collision.transform.position.x) <= 0.1f)
        {
            isPassDay = true;
            GameManager.Instance.player.hasSleptInBed = true;
            GameManager.Instance.dayTimeManager.NextDay();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            isPassDay = false;
    }
}
