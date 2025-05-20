
using UnityEngine;

public enum ToolType
{
    None,
    Hoe,
    Pickaxe,
    Axe,
    FishingRod,
    WateringCan
}

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
        if (stateMachine.currentState != player.workingState && player.toolType != ToolType.None
            && Input.GetMouseButtonDown(0) && !GameManager.Instance.uiManager.inventoryPanel.activeSelf)
        {
            player.InteractWithTile();
            stateMachine.ChangeState(player.workingState);
        }
    }

    public virtual void ExitState()
    {
        player.anim.SetBool(animBoolName, false);
    }
}