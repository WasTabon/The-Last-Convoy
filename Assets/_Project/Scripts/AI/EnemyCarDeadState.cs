using UnityEngine;

public class EnemyCarDeadState : EnemyCarState
{
    public override void Enter()
    {
        Controller.OnDeath();
    }

    public override void FixedUpdate()
    {
    }
}
