using System.Collections.Generic;
using Components;
using Unity.Entities;
using UnityEngine.InputSystem;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class PlayerInputSystem : SystemBase
{
    private PlayerControls _playerControls;
    private float _sfxTimer;

    protected override void OnCreate()
    {
        _playerControls = new PlayerControls();
    }

    protected override void OnStartRunning()
    {
        _playerControls.Enable();
    }

    protected override void OnStopRunning()
    {
        _playerControls.Disable();
    }

    protected override void OnDestroy()
    {
        _playerControls.Dispose();
    }

    protected override void OnUpdate()
    {
        if (!GameData.Weapon.Equals("Rifle")) return;
        if (PauseLogic.isPaused) return;

        bool firing = _playerControls.Player.Fire.IsPressed();

        float dt = SystemAPI.Time.DeltaTime;

        _sfxTimer -= dt;
        
        foreach (var weapon in SystemAPI.Query<RefRW<Weapon>>().WithAll<WeaponTag>())
        {
            weapon.ValueRW.IsFiring = firing;
            
            if (firing && _sfxTimer <= 0f)
            {
                SFXManager.Instance.ShootBullet();
                _sfxTimer = 1f / weapon.ValueRO.FireRate;
            }
        }
    }
}