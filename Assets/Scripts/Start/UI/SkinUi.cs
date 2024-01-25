using System;
using System.Collections.Generic;
using Game.General;
using TMPro;
using UnityEngine;

namespace Start.UI
{
    public class SkinUi : MonoBehaviour
    {
        [SerializeField] private GameObject monsterPanel, survivorPanel;
        [SerializeField] private GameObject[] survivorFrames, monsterFrames;
        [SerializeField] private Transform mainPoint, backPoint, survivor, monster;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private GameObject coinIcon, selectButton;
        [SerializeField] private int[] survivorCosts, monsterCosts;
        private bool survivorIsMain;
        private int selectedIndex;
        private readonly List<SurvivorSkin> survivorSkins = new List<SurvivorSkin>();
        private readonly List<MonsterSkin> monsterSkins = new List<MonsterSkin>();

        private void Start()
        {
            foreach (var s in FindObjectsOfType<SurvivorSkin>())
            {
                survivorSkins.Add(s);
            }
            foreach (var s in FindObjectsOfType<MonsterSkin>())
            {
                monsterSkins.Add(s);
            }
            SetSurvivorSkin(PlayerPrefs.GetInt("Survivor Skin",0));
            Switch();
        }

        private int GetSkinIndex()
        {
            return survivorIsMain ? PlayerPrefs.GetInt("Survivor Skin", 0) : PlayerPrefs.GetInt("Monster Skin", 0);
        }

        private void SetButton()
        {
            selectButton.SetActive(selectedIndex != GetSkinIndex());
            if (PlayerPrefs.GetInt("IHaveThisSkin" + selectedIndex + survivorIsMain, selectedIndex == 0? 1:0) == 1)
            {
                costText.text = "Select";
                coinIcon.SetActive(false);
                costText.color = Color.white;
            }
            else
            {
                var cost = survivorIsMain ? survivorCosts[selectedIndex] : monsterCosts[selectedIndex];
                costText.text = cost.ToString();
                costText.color = Coins.IsCoinEnough(cost)  ? Color.green : Color.red;
                coinIcon.SetActive(true);
            }
        }
        

        #region Buttons
        public void SetSurvivorSkin(int index)
        {
            for (int i = 0; i < survivorSkins.Count; i++)
            {
                survivorSkins[i].SetSkin(index);
            }

            for (int i = 0; i < survivorFrames.Length; i++)
            {
                survivorFrames[i].SetActive( i == index);
            }
            selectedIndex = index;
            SetButton();
        }
        public void SetMonsterSkin(int index)
        {
            for (int i = 0; i < monsterSkins.Count; i++)
            {
                monsterSkins[i].SetSkin(index);
            }

            for (int i = 0; i < monsterFrames.Length; i++)
            {
                monsterFrames[i].SetActive( i == index);
            }
            selectedIndex = index;
            SetButton();
        }
        public void Switch()
        {
            if (survivorIsMain)
            {
                survivorPanel.SetActive(false);
                monsterPanel.SetActive(true);
                survivor.position = backPoint.position;
                monster.position = mainPoint.position;
            }
            else
            {
                survivorPanel.SetActive(true);
                monsterPanel.SetActive(false);
                survivor.position = mainPoint.position;
                monster.position = backPoint.position;
            }
            survivorIsMain = !survivorIsMain;
            for (int i = 0; i < monsterFrames.Length; i++)
            {
                monsterFrames[i].SetActive(i == GetSkinIndex());
            }
            for (int i = 0; i < survivorFrames.Length; i++)
            {
                survivorFrames[i].SetActive(i == GetSkinIndex());
            }
            selectedIndex = GetSkinIndex();
            SetSavedSkin();
            SetButton();
        }
        public void SetSavedSkin()
        {
            var savedSkinSurvivorIndex = PlayerPrefs.GetInt("Survivor Skin", 0);
            var savesMonsterSkinIndex = PlayerPrefs.GetInt("Monster Skin", 0); 
            for (int i = 0; i < survivorSkins.Count; i++)
            {
                survivorSkins[i].SetSkin(savedSkinSurvivorIndex);
            }
            for (int i = 0; i < monsterSkins.Count; i++)
            {
                monsterSkins[i].SetSkin(savesMonsterSkinIndex);
            }
        }
        public void SelectOrByu()
        {
            if (PlayerPrefs.GetInt("IHaveThisSkin" + selectedIndex + survivorIsMain, selectedIndex == 0?1:0) == 0)
            {
                var cost = survivorIsMain ? survivorCosts[selectedIndex] : monsterCosts[selectedIndex];
                if (!Coins.IsCoinEnough(cost, true)) return;
                PlayerPrefs.SetInt("IHaveThisSkin" + selectedIndex + survivorIsMain, 1);
            }
            else
            {
                if(survivorIsMain)PlayerPrefs.SetInt("Survivor Skin", selectedIndex);
                else PlayerPrefs.SetInt("Monster Skin", selectedIndex); 
            }
            SetButton();
        }
        #endregion
    }
}
