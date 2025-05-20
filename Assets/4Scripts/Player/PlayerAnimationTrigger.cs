using UnityEngine;

public class PlayerAnimationTrigger : MonoBehaviour
{
    private void ChangeStateTrigger()
    {
        Player player = GetComponentInParent<Player>();
        player.stateMachine.ChangeState(player.idleState);
    }
}
