using UnityEngine;

public class EnemyCarAttackingState : EnemyCarState
{
    private float _lastShotTime;

    public override void Enter()
    {
        _lastShotTime = Time.time;
        Controller.SetTargetSpeed(Controller.BaseSpeed * 0.8f);
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

        if (!Controller.IsPlayerInAttackRange())
        {
            Controller.ChangeState(Controller.DrivingState);
            return;
        }

        TryShoot();
    }

    private void TryShoot()
    {
        if (Time.time - _lastShotTime >= Controller.FireRate)
        {
            Controller.Shoot();
            _lastShotTime = Time.time;
        }
    }
}
