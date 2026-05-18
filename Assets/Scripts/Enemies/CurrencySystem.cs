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
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        int totalCoinsThisFrame = 0;
        int totalSoulsThisFrame = 0;

        foreach (var (reward, entity) in SystemAPI.Query<RefRO<CurrencyRewardComponent>>().WithAll<DeadTag>().WithEntityAccess())
        {
            totalCoinsThisFrame += reward.ValueRO.Coins;
            totalSoulsThisFrame += reward.ValueRO.Souls;

            ecb.DestroyEntity(entity);
        }

        if (totalCoinsThisFrame > 0 || totalSoulsThisFrame > 0)
        {
            if (PlayerWallet.Instance != null)
            {
                PlayerWallet.Instance.AddRewards(totalCoinsThisFrame, totalSoulsThisFrame);
            }
        }
    }
}