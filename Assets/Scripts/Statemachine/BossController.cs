using Components;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using static UnityEngine.UI.Image;

public class BossController : MonoBehaviour
{
    private StateMachine _stateMachine;

    public TiggerInBossArena triggerArena;

    //for movement
    public float width = 3f;
    public float height = 1.5f;
    public float speed = 1f;
    public Vector3 origin;
    private float angle;

    public Transform player;

    public GameObject laserPrefab;

    public EntityManager em;
    private void Awake()
    {
        _stateMachine = new StateMachine();
    }

    private void Start()
    {
        
        player = GameObject.FindWithTag("Player").transform;

        _stateMachine.SetState(new IdleState(this));


        var em = World.DefaultGameObjectInjectionWorld.EntityManager;
        if (em == null) return;

        
        var query = em.CreateEntityQuery(typeof(BossPortalPhaseActive));
        if (!query.IsEmpty)
        {
            em.DestroyEntity(query);
        }
        
    }

    private void FixedUpdate()
    {
        if (!triggerArena.playerInside) return;

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