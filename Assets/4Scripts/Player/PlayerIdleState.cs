using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)
        : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void EnterState()
    {
        player.anim.SetBool(animBoolName, false);
    }

    public override void UpdateState()
    {
        if (player.moveInput.magnitude > 0)
        {
            stateMachine.ChangeState(player.moveState);
        }
        base.UpdateState();
    }

    public override void ExitState()
    {
    }
}