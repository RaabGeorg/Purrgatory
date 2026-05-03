using System;
using UnityEngine;
using UnityEngine.InputSystem;
 
public class oldWeapon : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;  
    [SerializeField] private Transform firePoint;          
    [SerializeField] private float fireRate = 0.2f;        
 
    private float nextFireTime = 0f;
    private PlayerControls playerControls;

    void Start()
    {
        playerControls = new PlayerControls();
        playerControls.Player.Enable();
    }

    void Update()
    {
        if (playerControls.Player.Fire.IsPressed() &&
            Time.time >= this.nextFireTime)
        {
            this.Shoot();
            this.nextFireTime = Time.time + this.fireRate;
        }
    }
 
    private void Shoot()
    {
        if (this.projectilePrefab == null || this.firePoint == null)
        {
            Debug.LogWarning("Weapon: projectilePrefab or firePoint not assigned!");
            return;
        }
        Instantiate(this.projectilePrefab, this.firePoint.position, this.firePoint.rotation);
    }
    
}