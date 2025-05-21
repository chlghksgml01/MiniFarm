using UnityEngine;

public class PlayerWorkingState : PlayerState
{
    public PlayerWorkingState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)
        : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void EnterState()
    {
        player.anim.SetInteger(animBoolName, (int)player.toolType);
    }

    public override void UpdateState()
    {
        base.UpdateState();
    }

    public override void ExitState()
    {
        player.anim.SetInteger(animBoolName, (int)ToolType.None);
    }
}
