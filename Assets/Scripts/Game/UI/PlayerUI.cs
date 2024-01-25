using System;
using System.Collections;
using Game.Monster;
using Game.Survivors;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class PlayerUI : MonoBehaviour
    { 
        public static PlayerUI Instance {get; private set;} 
        [SerializeField] private GameObject monsterPanel, survivorPanel;
        private NetworkMonster networkMonster;
        private NetworkSurvivor networkSurvivor;
        
        [Header("Survivor")] 
        [SerializeField] private Image countdown;
        [SerializeField] private GameObject reviveButton, generatorButton, controllerPanel, switchViewPanel;
        
        [Header("Monster")] 
        [SerializeField] private GameObject holdButton;
        [SerializeField] private GameObject releaseButton, putInCageButton;

        
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            monsterPanel.SetActive(false);
            survivorPanel.SetActive(false);
            countdown.gameObject.SetActive(false);
            reviveButton.SetActive(false);
            holdButton.SetActive(false);
            reviveButton.SetActive(false);
            generatorButton.SetActive(false);
            putInCageButton.SetActive(false);
            controllerPanel.SetActive(true);
            switchViewPanel.SetActive(false);
        }

        public void SetMonster(NetworkMonster networkMonster)
        {
            monsterPanel.SetActive(true);
            this.networkMonster = networkMonster;
        }
        public void SetSurvivor(NetworkSurvivor networkSurvivor)
        {
            survivorPanel.SetActive(true);
            this.networkSurvivor = networkSurvivor;
        }


        public void HideCountdown() => countdown.gameObject.SetActive(false);
        public IEnumerator StartReviveCountDown(float reviveTime)
        {
            countdown.gameObject.SetActive(true);
            var remainTime = reviveTime;
            while (true)
            {
                remainTime -= Time.deltaTime;
                if(remainTime <= 0)break;
                countdown.fillAmount = 1 - remainTime / reviveTime;
                yield return null;
            }
            countdown.gameObject.SetActive(false);
            networkSurvivor.SetSurvivorState(SurvivorState.Normal);
        }
        public IEnumerator StartFreeCountDown(float reviveTime)
        {
            countdown.gameObject.SetActive(true);
            var remainTime = reviveTime;
            while (true)
            {
                remainTime -= Time.deltaTime;
                if (remainTime <= 0) break;
                countdown.fillAmount = 1 - remainTime / reviveTime;
                yield return null;
            }
            countdown.gameObject.SetActive(false);
            networkSurvivor.FreeAllyEnd();
        }
        public IEnumerator StartReviveCountDown(float reviveTime,NetworkSurvivor ally)
        {
            countdown.gameObject.SetActive(true);
            var remainTime = reviveTime;
            while (true)
            {
                remainTime -= Time.deltaTime;
                if (remainTime <= 0) break;
                countdown.fillAmount = 1 - remainTime / reviveTime;
                if (ally.Hold()) break;
                yield return null;
            }
            countdown.gameObject.SetActive(false);
            networkSurvivor.ReviveAlly();
        }
      

        private void Update()
        {
            ButtonsVisibility();
        }

        private void ButtonsVisibility()
        {
            if (networkSurvivor != null) reviveButton.SetActive(networkSurvivor.ShowReviveButton());
            if (networkMonster != null)
            {
                holdButton.SetActive(networkMonster.ShowHoldButton());
                releaseButton.SetActive(networkMonster.ShowReleaseButton());
                putInCageButton.SetActive(networkMonster.ShowCageButton());
            }
        }
        public void ShowGeneratorButton(bool show) => generatorButton.SetActive(show);

        public void ShowSwitchPanel()
        {
            switchViewPanel.SetActive(true);
            controllerPanel.SetActive(false);
        }
        
        #region Buttons
        public void MonsterAttack()
        {
            networkMonster.Attack();
        }
        public void SurvivorCrouched()
        {
            var survivorState = networkSurvivor.SurvivorState;
            if(survivorState == SurvivorState.Crawling)return;
            var state = survivorState == SurvivorState.Crouched ? SurvivorState.Normal : SurvivorState.Crouched;
            networkSurvivor.SetSurvivorState(state);
        }
        public void Revive()
        {
            networkSurvivor.Revive();
        }
        public void HoldSurvivor()
        {
           networkMonster.HoldSurvivor();
        }

        public void ReleaseButton()
        {
            networkMonster.ReleaseSurvivor();
        }

        public void FixGenerator()
        {
            networkSurvivor.FixGeneratorOnline();
        }

        public void PutInCage()
        {
            networkMonster.PutSurvivorInCage();
        }

        public void SwitchView(int index)
        {
            networkSurvivor.SwitchView(index);
        }
        #endregion
    }
}
