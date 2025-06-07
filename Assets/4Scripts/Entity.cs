using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Info")]

    [SerializeField] public float speed;
    [SerializeField] public int damage;

    [Header("FlashFX")]
    [SerializeField] private float flashDuration;
    [SerializeField] private Material hitMat;

    private SpriteRenderer sprite;
    private Material originalMat;
    protected Coroutine flashCoroutine;

    protected virtual void OnEnable()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        if (sprite == null)
            sprite = GetComponent<SpriteRenderer>();
        originalMat = sprite.material;

        if (sprite == null || originalMat == null || hitMat == null)
        {
            Debug.LogError("Entity - 머테리얼 없음");
        }
    }

    protected IEnumerator FlashFX()
    {
        sprite.material = hitMat;
        Color currentColor = sprite.color;
        sprite.color = Color.white;

        yield return new WaitForSeconds(flashDuration);

        sprite.color = currentColor;
        sprite.material = originalMat;
    }
}