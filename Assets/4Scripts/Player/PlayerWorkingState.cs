using System;
using UnityEngine;

public class PlayerWorkingState : PlayerState
{
    public PlayerWorkingState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)
        : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void EnterState()
    {
        player.anim.SetInteger(animBoolName, (int)player.playerToolType);
        player.moveInput = Vector3.zero;
        player.UseStamina();

        if (player.playerToolType == ToolType.Sword)
        {
            player.SetPlayerDirection(GameManager.Instance.tileManager.mouseDirection);

            if (player.playerDir == playerDir.Left)
            {
                player.SwordColliderLeft.SetActive(true);
            }
            else if (player.playerDir == playerDir.Right)
            {
                player.SwordColliderRight.SetActive(true);
            }

            if (player.playerDir == playerDir.Down)
            {
                player.SwordColliderDown.SetActive(true);
            }
            else if (player.playerDir == playerDir.Up)
            {
                player.SwordColliderUp.SetActive(true);
            }
        }
    }

    public override void UpdateState()
    {
        base.UpdateState();

    }

    public override void ExitState()
    {
        player.anim.SetInteger(animBoolName, (int)ToolType.None);
        if (player.playerToolType == ToolType.Sword)
        {
            player.SwordColliderLeft.SetActive(false);
            player.SwordColliderRight.SetActive(false);
            player.SwordColliderDown.SetActive(false);
            player.SwordColliderUp.SetActive(false);
        }
    }
}
