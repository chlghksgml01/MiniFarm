
using UnityEngine;

public class PlayerState
{
    protected Player player;
    protected PlayerStateMachine stateMachine;
    protected string animBoolName;

    public PlayerState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)
    {
        player = _player;
        stateMachine = _stateMachine;
        animBoolName = _animBoolName;
    }

    public virtual void EnterState()
    {
        player.anim.SetBool(animBoolName, true);
    }

    public virtual void UpdateState()
    {
    }

    public virtual void ExitState()
    {
        player.anim.SetBool(animBoolName, false);
    }
}