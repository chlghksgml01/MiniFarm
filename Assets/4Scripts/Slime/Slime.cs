using System.Collections;
using UnityEngine;

public enum SlimeState
{
    Patrol,
    Angry,
    Death
}

public class Slime : MonoBehaviour
{
    [SerializeField] public float speed = 1f;
    [SerializeField] public float angrySpeed = 1.5f;
    [SerializeField] private float patrolRadius = 2f;
    [SerializeField] private float detectionRange = 3f;
    [SerializeField] private int hp = 10;

    [HideInInspector] public Vector3 moveInput;
    [HideInInspector] public Animator anim;

    private SlimeState slimeState = SlimeState.Patrol;

    private Transform playerTransform;
    private Coroutine patrolCoroutine;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();

        if (anim == null)
            Debug.Log("Slime - anim ¾øÀ½");
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

    private void Patrol()
    {
        CheckAngryState();
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
            transform.Translate(moveInput * angrySpeed * Time.deltaTime);
        }
    }

    private void CheckAngryState()
    {
        if (Vector3.Distance(playerTransform.position, transform.position) <= detectionRange)
        {
            StopCoroutine(patrolCoroutine);
            patrolCoroutine = null;

            slimeState = SlimeState.Angry;
            anim.SetBool("isPlayerDetected", true);
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
                transform.Translate(moveInput * speed * Time.deltaTime);
                yield return null;
            }
        }
    }
}
