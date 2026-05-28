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
        
        int totalCoins = 0;
        int totalSouls = 0;

        foreach (var (reward, entity) in SystemAPI.Query<RefRO<CurrencyRewardComponent>>().WithAll<MarkedForExecution>().WithEntityAccess())
        {
            totalCoins += reward.ValueRO.Coins;
            totalSouls += reward.ValueRO.Souls;

            ecb.AddComponent<Executed>(entity);
        }

        if ((totalCoins > 0 || totalSouls > 0) && PlayerWallet.Instance != null)
        {
            PlayerWallet.Instance.AddRewards(totalCoins, totalSouls);
        }
    }
}