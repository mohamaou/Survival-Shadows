using System;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class WinPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI coins, addedCoinsText;
        private int coinsToAdd;
        

        private void Awake()
        {
            coins.text = PlayerPrefs.GetInt("Coins", 100).ToString();
        }

        private void Start()
        {
            if (GameManager.Instance.GetMonster() != null)
            {
                coinsToAdd = 500;
            }
            else
            {
                var generatorsCount = GameManager.Instance.GetSurvivor().GetTheGeneratorFixedNumber();
                coinsToAdd = generatorsCount switch
                {
                    0 => 300,
                    1 => 400,
                    2 => 500,
                    3 => 600,
                    4 => 700,
                };
            }
            addedCoinsText.text = "+" +coinsToAdd;
            var c = PlayerPrefs.GetInt("Coins", 100);
            PlayerPrefs.SetInt("Coins",c + coinsToAdd);
            coins.text = (c + coinsToAdd).ToString();
        }
    }
}
