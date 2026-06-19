using UnityEngine;

public class ExampleState : IState
{
    private readonly BossController _boss;

    public ExampleState(BossController boss)
    {
        _boss = boss;
    }

    public void Enter()
    {
        Debug.Log("enter");
        //so like right here this gets called when u switch to this state
    }

    public void Tick()
    {
        //basically update but u also like call the switchstate stuff here and once u do that exit gets called automatically yknowyknow
       
    }

    public void Exit()
    {
        Debug.Log("exit");
        //here cleanup shit when exit state yes
    }
}
