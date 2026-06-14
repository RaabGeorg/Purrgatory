using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    public static PlayerWallet Instance { get; private set; }

    [Header("Current Balances")]
    [SerializeField] private int condensedSouls;
    [SerializeField] private int souls;

    public int CondensedSouls => condensedSouls;
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

    public void AddRewards(int condensedSoulsEarned, int soulsEarned)
    {
        
        condensedSouls += condensedSoulsEarned;
        souls += soulsEarned;

        //Debug.Log($"Wallet Updated -> CondensedSouls: {condensedSouls} (+{condensedSoulsEarned}), Souls: {souls} (+{soulsEarned})");
    }

    public bool SpendCoins(int amount)
    {
        if (condensedSouls < amount) return false;
        condensedSouls -= amount;
        return true;
    }

    public bool SpendSouls(int amount)
    {
        if (souls < amount) return false;
        souls -= amount;
        return true;
    }
}