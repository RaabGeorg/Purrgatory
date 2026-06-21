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
   public bool isRecharging;
   public float _rechargeProgress;
    
    private float rechargeStartTime;
    [SerializeField] private AudioSource footstepSource;
    
    
    
    
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
            velocity.y = 0f;
        }
        else
        {
            velocity.y = -2f;
        }
        
        Vector2 move = playerControls.Player.Move.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(move.x, 0, move.y);
        SoundState(moveDirection);
        
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
            SFXManager.Instance.PlayDash();
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
        float elapsed = 0f;
        while (elapsed < dashRecharge)
        {
            elapsed += Time.deltaTime;
            _rechargeProgress = elapsed / dashRecharge;
            yield return null;
        }
        _rechargeProgress = 0f;
        isRecharging = false;
        if (dashCount < 2)
            dashCount += 1;
    }
    
    private IEnumerator DashCooldown(float dashCooldown)
    {
        yield return new WaitForSeconds(dashCooldown) ;
        isDashing = false;
    }

    public void UpdateStats()
    {
        speed = PlayerStatsManager.Instance.stats.baseMoveSpeed.Value;
    }
    
    public void SoundState(Vector3 moveDirection)
    {
        bool isMoving = moveDirection.sqrMagnitude > 0.01f;

        if (isMoving && !footstepSource.isPlaying)
        {
            footstepSource.Play();
        }
        else if (!isMoving && footstepSource.isPlaying)
        {
            footstepSource.Stop();
        }
    }
}