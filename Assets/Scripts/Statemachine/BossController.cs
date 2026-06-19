using UnityEngine;

public class BossController : MonoBehaviour
{
    private StateMachine _stateMachine;

    // Add shit here your states will need here
    
    public Transform[] waypoints;
    public float moveSpeed = 5f;

    private void Awake()
    {
        _stateMachine = new StateMachine();
    }

    private void Start()
    {
        // u set initialstate here u know the start state Ig
        // _stateMachine.SetState(new initialStateTypeShit(this));
        _stateMachine.SetState(new BossPortalAttackState(this));
    }

    private void Update()
    {
        _stateMachine.Tick();
    }

    //with this u switch states everywhere
    public void SwitchState(IState newState)
    {
        _stateMachine.SetState(newState);
    }
}