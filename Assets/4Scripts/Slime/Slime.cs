using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private float patrolRadius = 2f;

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

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        if (anim == null)
            Debug.Log("Slime - anim ¾øÀ½");
    }

    private void Start()
    {
        StartCoroutine(Patrol());
        anim.SetFloat("vertical", 0f);
        anim.SetFloat("horizontal", 0f);
    }

    private void Update()
    {
        if (slimeState == SlimeState.Death | slimeState == SlimeState.Idle)
            return;

        transform.Translate(moveInput * speed * Time.deltaTime);
    }


    private IEnumerator Patrol()
    {
        Vector3 startPos = transform.position;
        while (true)
        {
            float waitTime = Random.Range(1f, 3f);

            SetState(SlimeState.Idle, false);

            yield return new WaitForSeconds(waitTime);

            SetState(SlimeState.Move, true);

            Vector3 distance = Random.insideUnitCircle * patrolRadius;
            Vector3 targetPos = startPos + distance;

            while (Vector3.Distance(transform.position, targetPos) > 0.1f)
            {
                moveInput = (targetPos - transform.position).normalized;
                anim.SetFloat("horizontal", moveInput.x);
                anim.SetFloat("vertical", moveInput.y);
                yield return null;
            }
        }
    }

    private void SetState(SlimeState _slimeState, bool isPlay)
    {
        slimeState = _slimeState;
        anim.SetBool(animStateDict[_slimeState], isPlay);
    }
}
