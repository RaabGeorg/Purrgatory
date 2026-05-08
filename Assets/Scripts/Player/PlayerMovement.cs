using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private float speed;
    private float dashSpeed;
    private float _dashTime = 0.25f;
    private float startTime;
    public float dashCooldown = 3;
    private bool isDashing;

    public CharacterStats stats;
    public WeaponStats Weapon;
    
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
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    void Start()
    {
        startTime = -dashCooldown;
        controller = GetComponent<CharacterController>();
        speed = stats.baseMoveSpeed.Value;
        dashSpeed = speed * 5;
    }

    void Update()
    {
        Vector2 move = playerControls.Player.Move.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(move.x, 0, move.y);

        controller.Move(moveDirection * (speed * Time.deltaTime));

        

        if (playerControls.Player.Dash.WasPressedThisFrame() && !isDashing)
        {
            StartCoroutine(DashCooldown(dashCooldown));
            StartCoroutine(DashRoutine(moveDirection));
        }

    }

    private IEnumerator DashRoutine(Vector3 dashDirection)
    {
        isDashing = true;
        startTime = Time.time;
        while (Time.time < startTime + _dashTime )
        {
            controller.Move(dashDirection * (dashSpeed * Time.deltaTime));
            yield return null;
        }
    }

    private IEnumerator DashCooldown(float dashCooldown)
    {
        yield return new WaitForSeconds(dashCooldown) ;
        isDashing = false;
    }

    public float GetDashTimer()
    {
        float elapsedTime = Time.time - startTime;
        float remainingTime = dashCooldown - elapsedTime;
        return Mathf.Max(0, remainingTime);
    }
    
}