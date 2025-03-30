
public interface IPlayerState
{
    public void EnterState(Movement movement);
    public void UpdateState(Movement movement);
    public void ExitState(Movement movement);
}

public class PlayerIdleState : IPlayerState
{
    public void EnterState(Movement movement)
    {
        movement.SetAnimBool("isMoving", false);
    }

    public void UpdateState(Movement movement)
    {
        if (movement.MoveInput.magnitude > 0)
        {
            movement.ChangeState(movement.moveState);

            movement.SetAnimBool("isMoving", true);
        }
    }

    public void ExitState(Movement movement)
    {
        movement.SetAnimBool("isMoving", true);
    }
}

public class PlayerMoveState : IPlayerState
{
    public void EnterState(Movement movement)
    {
        movement.SetAnimBool("isMoving", true);
    }

    public void UpdateState(Movement movement)
    {
        if (movement.MoveInput.magnitude > 0)
        {
            movement.SetAnimFloat("horizontal", movement.MoveInput.x);
            movement.SetAnimFloat("vertical", movement.MoveInput.y);
        }

        else if (movement.MoveInput.magnitude == 0)
        {
            movement.ChangeState(movement.idleState);
        }
    }

    public void ExitState(Movement movement)
    {
        movement.SetAnimBool("isMoving", false);
    }
}