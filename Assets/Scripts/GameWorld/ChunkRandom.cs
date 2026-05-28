public static class ChunkRandom
{
    public static readonly uint SessionSeed = (uint)System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public static float GetRotation(int x, int z)
    {
        var random = Unity.Mathematics.Random.CreateFromIndex((uint)(x * 73856093 ^ z * 19349663) ^ SessionSeed);
        return random.NextInt(0, 4) * 90f;
    }

    public static float GetYOffset(int x, int z)
    {
        var random = Unity.Mathematics.Random.CreateFromIndex((uint)(x * 53856093 ^ z * 23349663) ^ SessionSeed);
        return random.NextFloat(-0.01f, 0.01f);
    }
}