using System.Collections;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float speed;
    private float dashSpeed;
    private float _dashTime = 0.25f;
    private float startTime;
    public float dashRecharge = 5;
    public float dashCooldown = 0.5f;
    public float dashCount = 2f;
    private bool isDashing;
    private bool isRecharging;

    private PlayerControls playerControls;
    
    // Tracks the active dash velocity to combine with standard movement
    private Vector3 dashVelocity = Vector3.zero; 

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
        GameEvents.OnStatsChanged += UpdateStats;
    }

    private void OnDisable()
    {
        playerControls.Disable();
        GameEvents.OnStatsChanged -= UpdateStats;
    }

    private void OnDestroy()
    {
        playerControls?.Dispose();
    }

    void Start()
    {
        startTime = -dashCooldown;
        speed = PlayerStatsManager.Instance.stats.baseMoveSpeed.Value;
        dashSpeed = speed * 5;
    }

    void Update()
    {
        Vector2 move = playerControls.Player.Move.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(move.x, 0, move.y);
        
        Vector3 standardVelocity = moveDirection * speed;

        ApplyVelocityToECS(standardVelocity + dashVelocity);

        if (dashCount < 2 && !isRecharging)
        {
            isRecharging = true;
            StartCoroutine(DashRecharge(dashRecharge));
        }

        if (playerControls.Player.Dash.WasPressedThisFrame() && !isDashing && dashCount > 0)
        {
            StartCoroutine(DashCooldown(dashCooldown));
            StartCoroutine(DashRoutine(moveDirection));
            SFXManager.Instance.PlayDash();
        }
    }

    private void ApplyVelocityToECS(Vector3 targetVelocity)
    {
        if (PlayerBridge.Instance == null) return;
        
        var em = PlayerBridge.Instance.GetEntityManager();
        var playerEnt = PlayerBridge.Instance.GetPlayerEntity();

        if (em.Exists(playerEnt) && em.HasComponent<PhysicsVelocity>(playerEnt))
        {
            var physVel = em.GetComponentData<PhysicsVelocity>(playerEnt);
            
            physVel.Linear = new float3(targetVelocity.x, targetVelocity.y, targetVelocity.z);
            
            em.SetComponentData(playerEnt, physVel);
        }
    }

    private IEnumerator DashRoutine(Vector3 dashDirection)
    {
        isDashing = true;
        startTime = Time.time;
        dashCount -= 1;
        
        if (dashDirection == Vector3.zero) dashDirection = transform.forward;
        
        dashDirection.y = 0f;
        
        dashDirection = dashDirection.normalized;

        while (Time.time < startTime + _dashTime)
        {
            
            dashVelocity = dashDirection * dashSpeed;
            yield return null;
        }
    
        dashVelocity = Vector3.zero;
    }

    private IEnumerator DashRecharge(float dashRecharge)
    {
        yield return new WaitForSeconds(dashRecharge);
        isRecharging = false;
        if (dashCount < 2)
        {
            dashCount += 1;
        }
    }
    
    private IEnumerator DashCooldown(float dashCooldown)
    {
        yield return new WaitForSeconds(dashCooldown);
        isDashing = false;
    }

    public float GetDashTimer()
    {
        float elapsedTime = Time.time - startTime;
        float remainingTime = dashCooldown - elapsedTime;
        return Mathf.Max(0, remainingTime);
    }

    public void UpdateStats()
    {
        speed = PlayerStatsManager.Instance.stats.baseMoveSpeed.Value;
        dashSpeed = speed * 5; // Ensure dash speed scales if base speed is upgraded
    }
}