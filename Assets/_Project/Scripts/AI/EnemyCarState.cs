public abstract class EnemyCarState
{
    protected EnemyCarController Controller;

    public void SetController(EnemyCarController controller)
    {
        Controller = controller;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
}
