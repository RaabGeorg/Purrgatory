using Components;
using UnityEngine;
using static UnityEngine.UI.Image;

public class BossController : MonoBehaviour
{
    private StateMachine _stateMachine;

    //for movement
    public float width = 3f;
    public float height = 1.5f;
    public float speed = 1f;
    public Vector3 origin;
    private float angle;

    public Transform player;

    public GameObject laserPrefab;
    private void Awake()
    {
        _stateMachine = new StateMachine();
    }

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        // u set initialstate here u know the start state Ig
        // _stateMachine.SetState(new initialStateTypeShit(this));
        _stateMachine.SetState(new IdleState(this));
    }

    private void FixedUpdate()
    {
        LookPlayer();
        EllipsisMovement();
        _stateMachine.Tick();
    }

    //with this u switch states everywhere
    public void SwitchState(IState newState)
    {
        _stateMachine.SetState(newState);
    }

    public void SetRandomState()
    {
        int roll = Random.Range(0, 3);
        switch (roll)
        {
            case 0: _stateMachine.SetState(new IdleState(this)); break;
            case 1: _stateMachine.SetState(new LaserCircleState(this)); break;
            case 2: _stateMachine.SetState(new BossPortalAttackState(this)); break;
        }
    }

    void EllipsisMovement()
    {
        angle += speed * Time.deltaTime;
        transform.position = origin + new Vector3(Mathf.Cos(angle) * width, 0f, Mathf.Sin(angle) * height);
    }

    void LookPlayer() 
    {
        Vector3 lookDir = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(lookDir);
    }
}