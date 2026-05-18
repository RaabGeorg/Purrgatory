using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    public static PlayerWallet Instance { get; private set; }

    [Header("Current Balances")]
    [SerializeField] private int coins;
    [SerializeField] private int souls;

    public int Coins => coins;
    public int Souls => souls;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddRewards(int coinsEarned, int soulsEarned)
    {
        coins += coinsEarned;
        souls += soulsEarned;

        Debug.Log($"Wallet Updated -> Coins: {coins} (+{coinsEarned}), Souls: {souls} (+{soulsEarned})");
    }

    public bool SpendCoins(int amount)
    {
        if (coins < amount) return false;
        coins -= amount;
        return true;
    }

    public bool SpendSouls(int amount)
    {
        if (souls < amount) return false;
        souls -= amount;
        return true;
    }
}