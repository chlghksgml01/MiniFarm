using System.Runtime.CompilerServices;
using System.Transactions;
using UnityEngine;

public class Movement : MonoBehaviour
{
    float speed = 1f;

    Animator anim;

    Vector3 moveInput;
    public Vector3 MoveInput { get { return moveInput; } set { moveInput = value; } }

    public IPlayerState currentState;
    public PlayerIdleState idleState = new PlayerIdleState();
    public PlayerMoveState moveState = new PlayerMoveState();

    void Awake()
    {
        anim = transform.Find("Visuals").GetComponent<Animator>();

        currentState = idleState;
        currentState.EnterState(this);
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        currentState.UpdateState(this);
    }

    private void FixedUpdate()
    {
        transform.Translate(moveInput * speed * Time.fixedDeltaTime);
    }

    public void ChangeState(IPlayerState newState)
    {
        if (currentState == newState) return;

        currentState.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }

    public void SetAnimBool(string name, bool value)
    {
        anim.SetBool(name, value);
    }

    public void SetAnimFloat(string name, float value)
    {
        anim.SetFloat(name, value);
    }
}