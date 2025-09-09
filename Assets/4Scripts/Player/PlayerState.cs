using UnityEngine;

public enum ToolType
{
    None,
    Hoe,
    Pickaxe,
    Axe,
    WateringCan,
    Sword,
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
        // ���� ���
        if (Input.GetMouseButtonDown(0) && !InGameManager.Instance.uiManager.inventoryPanel.activeSelf
            && player.CanWork())
        {
            InGameManager.Instance.tileManager.ChangeTileState();
            stateMachine.ChangeState(player.workingState);
        }

        // ���� �ɱ�
        else if (Input.GetMouseButtonDown(1) && !InGameManager.Instance.uiManager.inventoryPanel.activeSelf
            && player.holdItem.IsCropSeedHold())
        {
            InGameManager.Instance.tileManager.ChangeTileState();
        }

        // �۹� ��Ȯ
        else if (Input.GetMouseButtonDown(1) && !InGameManager.Instance.uiManager.inventoryPanel.activeSelf
            && InGameManager.Instance.cropManager.CanHarvest())
        {
            //if (player.CanHarvest())
            //{
                player.HarvestCrop();
                InGameManager.Instance.cropManager.HarvestCrop();
            //}
        }
    }

    public virtual void ExitState()
    {
        player.anim.SetBool(animBoolName, false);
    }
}