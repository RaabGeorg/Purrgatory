using Components;
using Unity.Entities;

public partial struct CurrencySystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        
        int totalCondensedSouls = 0;
        int totalSouls = 0;
        int totalXp = 0;

        foreach (var (reward, entity) in SystemAPI.Query<RefRO<CurrencyRewardComponent>>().WithAll<MarkedForExecution>().WithEntityAccess())
        {
            totalCondensedSouls += reward.ValueRO.CondensedSouls;
            totalSouls += reward.ValueRO.Souls;
            totalXp += reward.ValueRO.Xp;

            ecb.AddComponent<Executed>(entity);
        }

        if ((totalCondensedSouls > 0 || totalSouls > 0) && PlayerWallet.Instance != null)
        {
            PlayerWallet.Instance.AddRewards(totalCondensedSouls, totalSouls);
        }
        if (totalXp > 0)
            GameEvents.OnXPGained?.Invoke(totalXp);
    }
}