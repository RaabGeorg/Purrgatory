using Components;
using Unity.Entities;
using UnityEngine.InputSystem;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class PlayerInputSystem : SystemBase
{
    private PlayerControls _playerControls;

    protected override void OnCreate()
    {
        _playerControls = new PlayerControls();
        _playerControls.Enable();
    }

    protected override void OnDestroy()
    {
        _playerControls.Disable();
    }

    protected override void OnUpdate()
    {
        if (PauseLogic.isPaused) return;
        bool firing = _playerControls.Player.Fire.IsPressed();

        foreach (var weapon in SystemAPI.Query<RefRW<Weapon>>().WithAll<WeaponTag>())
            weapon.ValueRW.IsFiring = firing;
    }
}