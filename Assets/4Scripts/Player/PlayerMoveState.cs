using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)
        : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void UpdateState()
    {
        if (player.moveInput.magnitude > 0)
        {
            player.anim.SetFloat("horizontal", player.moveInput.x);
            player.anim.SetFloat("vertical", player.moveInput.y);
        }

        else if (player.moveInput.magnitude == 0)
        {
            stateMachine.ChangeState(player.idleState);
        }

        base.UpdateState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }
}
