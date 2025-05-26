using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimationTrigger : MonoBehaviour
{
    private void ChangeStateTrigger(string stateName = "")
    {
        Player player = GetComponentInParent<Player>();
        player.stateMachine.ChangeState(player.idleState);

        if (stateName == "HoldItem")
        {
            player.anim.SetBool("isHoldItem", true);
        }
    }
}
