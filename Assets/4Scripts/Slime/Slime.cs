using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public enum SlimeState
{
    Patrol,
    Angry,
    Death
}

public class Slime : Entity
{
    [Header("Info")]
    [SerializeField] public float angrySpeed = 1.5f;
    [SerializeField] private float patrolRadius = 2f;
    [SerializeField] private Item[] dropItems;

    [Header("플레이어 감지")]
    [SerializeField] private float detectionRange = 3f;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackTime = 0.2f;
    [SerializeField] public float attackDelay = 0.1f;

    [HideInInspector] private Vector3 moveInput;
    [HideInInspector] private Animator anim;

    private Rigidbody2D rigid;

    public SlimeState slimeState = SlimeState.Patrol;

    private Transform playerTransform;
    private Coroutine patrolCoroutine;

    private bool isKnockedBack = false;
    private float knockbackTimer;

    private void Awake()
    {
        knockbackTimer = knockbackTime;

        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        patrolCoroutine = StartCoroutine(StartPatrol());

        anim.SetFloat("vertical", 0f);
        anim.SetFloat("horizontal", 0f);

        playerTransform = GameManager.Instance.player.transform;
    }

    private void Update()
    {
        if (slimeState == SlimeState.Death)
            return;

        anim.SetBool("isMoving", moveInput != Vector3.zero);

        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
                knockbackTimer = knockbackTime;
                rigid.linearVelocity = Vector2.zero;
            }
        }

        switch (slimeState)
        {
            case SlimeState.Patrol:
                Patrol();
                break;
            case SlimeState.Angry:
                Angry();
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (slimeState == SlimeState.Death)
            return;

        if (collision.CompareTag("PlayerAttack") && !isKnockedBack)
        {
            isKnockedBack = true;

            if (flashCoroutine == null)
                flashCoroutine = StartCoroutine(FlashFX());

            rigid.linearVelocity = Vector2.zero;
            Vector2 knockbackDirection = transform.position - collision.transform.position;
            rigid.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode2D.Impulse);

            hp -= GameManager.Instance.player.damage;
            if (hp <= 0)
            {
                Death();
            }
        }
    }

    private void Death()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }

        slimeState = SlimeState.Death;
        anim.SetTrigger("isDeath");

        rigid.linearVelocity = Vector2.zero;

        DropItem();
    }

    private void DropItem()
    {
        if (dropItems == null)
        {
            Debug.Log("Slime - DropItem 없음");
            return;
        }

        int itemRate = Random.Range(0, 100);
        int itemCount = Random.Range(0, 3);

        if (GetRandomDropItem() != null)
        {
            var item = Instantiate(GetRandomDropItem(), transform.position, Quaternion.identity);
            Item spawnItem = item.GetComponent<Item>();
            spawnItem.SpawnItem(false, true, transform.position, spawnItem.itemData, itemCount);
        }
    }

    private GameObject GetRandomDropItem()
    {
        float totalRate = 0f;
        foreach (var item in dropItems)
            totalRate += item.itemData.GetDropRate();

        float randomPoint = Random.Range(0f, totalRate);
        float cumulative = 0f;

        foreach (var dropItem in dropItems)
        {
            cumulative += dropItem.itemData.GetDropRate();
            if (randomPoint <= cumulative)
                return dropItem.gameObject;
        }
        return null;
    }

    private void Patrol()
    {
        if (Vector3.Distance(playerTransform.position, transform.position) <= detectionRange)
        {
            StopCoroutine(patrolCoroutine);
            patrolCoroutine = null;

            slimeState = SlimeState.Angry;
            anim.SetBool("isPlayerDetected", true);
        }
    }

    private void Angry()
    {
        if (Vector3.Distance(playerTransform.position, transform.position) > detectionRange)
        {
            anim.SetBool("isPlayerDetected", false);
            slimeState = SlimeState.Patrol;
            patrolCoroutine = StartCoroutine(StartPatrol());
            return;
        }

        if (Vector3.Distance(transform.position, playerTransform.position) > 0.1f)
        {
            moveInput = (playerTransform.position - transform.position).normalized;
            anim.SetFloat("horizontal", moveInput.x);
            anim.SetFloat("vertical", moveInput.y);
            if (!isKnockedBack)
                transform.Translate(moveInput * angrySpeed * Time.deltaTime);
        }
    }

    private IEnumerator StartPatrol()
    {
        Vector3 startPos = transform.position;
        while (true)
        {
            moveInput = Vector3.zero;
            float waitTime = Random.Range(1f, 3f);

            yield return new WaitForSeconds(waitTime);

            Vector3 distance = Random.insideUnitCircle * patrolRadius;
            Vector3 targetPos = startPos + distance;

            while (Vector3.Distance(transform.position, targetPos) > 0.1f)
            {
                moveInput = (targetPos - transform.position).normalized;
                anim.SetFloat("horizontal", moveInput.x);
                anim.SetFloat("vertical", moveInput.y);
                if (!isKnockedBack)
                    transform.Translate(moveInput * speed * Time.deltaTime);
                yield return null;
            }
        }
    }

    private void DeathAnimationTrigger()
    {
        Destroy(gameObject);
    }
}
