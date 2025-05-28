
using UnityEngine;

public class SlimeSpawnController : MonoBehaviour
{
    [Header("½ºÆù")]
    [SerializeField] private BoxCollider2D spawnBox;
    [SerializeField] private GameObject slimePrefab;

    private void OnEnable()
    {
        GameManager.Instance.dayTimeManager.SpawnSlime += SpawnSlime;
    }

    private void OnDisable()
    {
        GameManager.Instance.dayTimeManager.SpawnSlime -= SpawnSlime;
    }

    private void SpawnSlime()
    {
        Instantiate(slimePrefab, GetRandomPointInBox(spawnBox), Quaternion.identity);
    }

    private Vector3 GetRandomPointInBox(BoxCollider2D box)
    {
        Vector2 center = (Vector2)box.transform.position + box.offset;
        Vector2 size = box.size;

        float randomX = Random.Range(-size.x / 2f, size.x / 2f);
        float randomY = Random.Range(-size.y / 2f, size.y / 2f);

        Vector2 randomPoint = center + new Vector2(randomX, randomY);

        return randomPoint;
    }
}