using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 10f;

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
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector2 move = playerControls.Player.Move.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(move.x, 0f, move.y);

        controller.Move(moveDirection * speed * Time.deltaTime);
    }
}