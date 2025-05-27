using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum SlimeState
{
    Idle,
    Move,
    Angry,
    Death
}

public class Slime : MonoBehaviour
{
    [SerializeField] public float speed = 1f;
    [SerializeField] public float angrySpeed = 1.5f;
    [SerializeField] private float patrolRadius = 2f;
    [SerializeField] private float detectionRange = 3f;

    [HideInInspector] public Vector3 moveInput;
    [HideInInspector] public Animator anim;

    private SlimeState slimeState;
    private Dictionary<SlimeState, string> animStateDict = new Dictionary<SlimeState, string>
    {
        { SlimeState.Idle , "isMoving" },
        { SlimeState.Move , "isMoving" },
        { SlimeState.Angry , "isPlayerDetected" },
        { SlimeState.Death , "isDeath" },
    };

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
        patrolCoroutine = StartCoroutine(Patrol());

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
            case SlimeState.Idle:
                Idle();
                break;
            case SlimeState.Move:
                Move();
                break;
            case SlimeState.Angry:
                Angry();
                break;
        }
    }

    private void Idle()
    {
        CheckAngryState();
    }

    private void Move()
    {
        CheckAngryState();
    }

    private void Angry()
    {
        if (Vector3.Distance(playerTransform.position, transform.position) > detectionRange)
        {
            anim.SetBool("isPlayerDetected", false);
            slimeState = SlimeState.Idle;
            patrolCoroutine = StartCoroutine(Patrol());
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
            slimeState = SlimeState.Angry;
            anim.SetBool("isPlayerDetected", true);
        }
    }


    private IEnumerator Patrol()
    {
        Vector3 startPos = transform.position;
        while (true)
        {
            float waitTime = Random.Range(1f, 3f);

            slimeState = SlimeState.Idle;

            yield return new WaitForSeconds(waitTime);

            slimeState = SlimeState.Idle;

            Vector3 distance = Random.insideUnitCircle * patrolRadius;
            Vector3 targetPos = startPos + distance;

            while (Vector3.Distance(transform.position, targetPos) > 0.1f && slimeState == SlimeState.Move)
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
