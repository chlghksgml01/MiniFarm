using UnityEngine;

public class PlayerPickUpState : PlayerState
{
    public PlayerPickUpState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)
        : base(_player, _stateMachine, _animBoolName)
    {
    }
    public override void EnterState()
    {
        base.EnterState();
        player.moveInput = Vector3.zero;
    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {
        base.ExitState();
        if (player.holdItem != null && player.holdItem.IsCropSeedHold())
            player.anim.SetBool("isHoldItem", true);
    }
}
