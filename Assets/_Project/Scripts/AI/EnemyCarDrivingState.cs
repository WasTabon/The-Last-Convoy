using UnityEngine;

public class EnemyCarDrivingState : EnemyCarState
{
    public override void Enter()
    {
        Controller.SetTargetSpeed(Controller.BaseSpeed);
    }

    public override void FixedUpdate()
    {
        Controller.UpdateWaypointProgress();
        Controller.AdjustSpeedBasedOnPlayer();
        Controller.UpdateTargetDirection();

        if (Controller.IsGrounded())
        {
            Controller.ApplyMovement();
            Controller.ApplyRotation();
        }

        Controller.ApplyExtraGravity();

        if (Controller.IsPlayerInAttackRange())
        {
            Controller.ChangeState(Controller.AttackingState);
        }
    }
}
