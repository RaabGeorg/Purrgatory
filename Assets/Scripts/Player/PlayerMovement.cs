using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
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
    public bool isRecharging { get; set; }
    private float rechargeStartTime;
    
    /*
    public CharacterStats stats;
    public WeaponStats Weapon;
    */
    private PlayerControls playerControls;

    private CharacterController controller;
    private Vector3 velocity;


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

    void Start()
    {
        startTime = -dashCooldown;
        controller = GetComponent<CharacterController>();
        speed = PlayerStatsManager.Instance.stats.baseMoveSpeed.Value;
        dashSpeed = speed * 5;
    }

    void Update()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        
        Vector2 move = playerControls.Player.Move.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(move.x, 0, move.y);
        
        controller.Move(moveDirection * (speed * Time.deltaTime));
        
        controller.Move(velocity * Time.deltaTime);

        if (dashCount < 2 && !isRecharging)
        {
            isRecharging = true;
            StartCoroutine(DashRecharge(dashRecharge));
        }

        if (playerControls.Player.Dash.WasPressedThisFrame() && !isDashing && dashCount > 0)
        {
            StartCoroutine(DashCooldown(dashCooldown));
            StartCoroutine(DashRoutine(moveDirection));
        }

        

    }

    private IEnumerator DashRoutine(Vector3 dashDirection)
    {
        isDashing = true;
        startTime = Time.time;
        dashCount -= 1;
        while (Time.time < startTime + _dashTime )
        {
            controller.Move(dashDirection * (dashSpeed * Time.deltaTime));
            yield return null;
        }
    }

    private IEnumerator DashRecharge(float dashRecharge)
    {
        isRecharging = true;
        rechargeStartTime = Time.time;
        
        yield return new WaitForSeconds(dashRecharge) ;
        
        isRecharging = false;
        if (dashCount < 2)
        {
            dashCount += 1;
        }
    }
    
    private IEnumerator DashCooldown(float dashCooldown)
    {
        yield return new WaitForSeconds(dashCooldown) ;
        isDashing = false;
    }

    public float GetRechargeProgress()
    {
        if (!isRecharging) return 0f;
        
        float elapsedTime = Time.time - rechargeStartTime;
        
        return Mathf.Clamp01(elapsedTime / dashCount);
    }

    public void UpdateStats()
    {
        speed = PlayerStatsManager.Instance.stats.baseMoveSpeed.Value;
    }
}