using System.Runtime.CompilerServices;
using System.Transactions;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float speed = 1f;

    Animator anim;
    SpriteRenderer sprite;

    int dir = 1;
    Vector3 moveInput;
    public Vector3 MoveInput { get { return moveInput; } }

    public IPlayerState currentState;
    public PlayerIdleState idleState = new PlayerIdleState();
    public PlayerMoveState moveState = new PlayerMoveState();

    void Awake()
    {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        currentState = idleState;
        currentState.EnterState(this);
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        if (moveInput.x < 0)
        {
            sprite.flipX = true;
            dir = -1;
        }
        else if (moveInput.x > 0)
        {
            sprite.flipX = false;
            dir = 1;
        }

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
}

public interface IPlayerState
{
    public void EnterState(Player player);
    public void UpdateState(Player player);
    public void ExitState(Player player);
}

public class PlayerIdleState : IPlayerState
{
    public void EnterState(Player player)
    {
        player.SetAnimBool("Move", false);
    }

    public void UpdateState(Player player)
    {
        if (player.MoveInput.magnitude > 0)
        {
            player.ChangeState(player.moveState);
        }
    }

    public void ExitState(Player player)
    {
        player.SetAnimBool("Move", true);
    }
}

public class PlayerMoveState : IPlayerState
{
    public void EnterState(Player player)
    {
        player.SetAnimBool("Move", true);
    }

    public void UpdateState(Player player)
    {
        if (player.MoveInput.magnitude == 0)
        {
            player.ChangeState(player.idleState);
        }
    }

    public void ExitState(Player player)
    {
        player.SetAnimBool("Move", false);
    }
}