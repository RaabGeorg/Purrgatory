using UnityEngine;

public class IdleState : IState
{
    private readonly BossController _boss;
    private float timer;
    private float cooldown;
    private float minCooldown = 2f;
    private float maxCooldown = 4f;

    public IdleState(BossController boss)
    {
        _boss = boss;
    }

    public void Enter()
    {
        cooldown = Random.Range(minCooldown, maxCooldown);
        timer = 0f;
    }

    public void Tick()
    {
        timer += Time.deltaTime;
        if (timer >= cooldown)
            _boss.SetRandomState();
    }

    public void Exit() { }
}