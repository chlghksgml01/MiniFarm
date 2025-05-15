
using UnityEngine;

public class PlayerState
{
    protected Player player;
    protected PlayerStateMachine stateMachine;
    protected string animBoolName;

    private ToolBar_UI toolBar;

    public PlayerState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)
    {
        player = _player;
        stateMachine = _stateMachine;
        animBoolName = _animBoolName;

        if (toolBar == null)
            toolBar = GameManager.Instance.uiManager.toolBarPanel.GetComponent<ToolBar_UI>();
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