using Unity.Burst;
using Unity.Entities;
using Components;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
public partial struct InvincibilitySystem : ISystem 
{
    public void OnUpdate(ref SystemState state)
    {
        float dt  = UnityEngine.Time.deltaTime;

        new TimerJob
        {
            DeltaTime = dt,
        }.ScheduleParallel();
    }




    [BurstCompile]
    public partial struct TimerJob : IJobEntity
    {
        public float DeltaTime;

        private void Execute(ref InvincibilityData invincibility)
        {
            if (invincibility.CurrentTime > 0f)
            {
                invincibility.CurrentTime -= DeltaTime;
            }
        }
    }
}
