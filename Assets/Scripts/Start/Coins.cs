using Start.UI;
using UnityEngine;

namespace Game.General
{
    public class Coins : MonoBehaviour
    {
        public static bool IsCoinEnough(int requiredAmount, bool useIt = false)
        {
            var coin = GetCoins();
            if (useIt && coin >= requiredAmount)
            {
                PlayerPrefs.SetInt("Coins", coin - requiredAmount);
                CoinUI.Instance.SetCoins(coin - requiredAmount);
            }
            return coin >= requiredAmount;
        }
        public static int GetCoins() => PlayerPrefs.GetInt("Coins", 100);

        public static void AddCoins(int amount)
        {
            var c = GetCoins();
            PlayerPrefs.SetInt("Coins",c + amount);
            CoinUI.Instance.SetCoins(GetCoins());
        } 
        
    }
}
