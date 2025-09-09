using UnityEngine;

public class PlayerAnimationTrigger : MonoBehaviour
{
    private void ChangeStateTrigger(string stateName = "")
    {
        Player player = GetComponentInParent<Player>();
        player.stateMachine.ChangeState(player.idleState);
    }
}
