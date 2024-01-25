using Game.General;
using TMPro;
using UnityEngine;

namespace Start.UI
{
    public class CoinUI : MonoBehaviour
    {
        public static CoinUI Instance;
        [SerializeField] private TextMeshProUGUI[] coinsText;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            SetCoins(Coins.GetCoins());
        }

        public void SetCoins(int coinAmount)
        {
            for (int i = 0; i < coinsText.Length; i++)
            {
                coinsText[i].text = coinAmount.ToString();
            }
        }
    }
}
