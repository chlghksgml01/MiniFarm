using UnityEngine;

public enum ToolType
{
    None,
    Hoe,
    Pickaxe,
    Axe,
    WateringCan,
    FishingRod,
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
        if (Input.GetMouseButtonDown(0) && !GameManager.Instance.uiManager.inventoryPanel.activeSelf
            && stateMachine.currentState != player.workingState && player.playerToolType != ToolType.None)
        {
            GameManager.Instance.tileManager.ChangeTileState();
            stateMachine.ChangeState(player.workingState);
        }

        else if (Input.GetMouseButtonDown(1) && !GameManager.Instance.uiManager.inventoryPanel.activeSelf
            && player.holdItem.IsCropSeedHold())
        {
            GameManager.Instance.tileManager.ChangeTileState();
        }

        else if ((Input.GetMouseButtonDown(1) && !GameManager.Instance.uiManager.inventoryPanel.activeSelf
            && GameManager.Instance.tileManager.CanHarvest()))
        {
            if (player.CanHarvest())
            {
                player.HarvestCrop();
                GameManager.Instance.tileManager.HarvestCrop();
            }
        }
    }

    public virtual void ExitState()
    {
        player.anim.SetBool(animBoolName, false);
    }
}