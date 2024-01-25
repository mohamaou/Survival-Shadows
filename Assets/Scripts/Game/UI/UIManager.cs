using System;
using System.Collections;
using System.Collections.Generic;
using Game.Monster;
using Game.Survivors;
using UnityEngine;

namespace Game.UI
{
    [Serializable]
    public class Panels
    {
        [SerializeField] private GameObject loadingPanel, startPanel, playPanel, losePanel, winPanel;

        public void SetPanel(GameState gameState)
        {
            loadingPanel.SetActive(false);
            startPanel.SetActive(false);
            playPanel.SetActive(false);
            losePanel.SetActive(false);
            winPanel.SetActive(false);
            switch (gameState)
            {
                case GameState.Loading: 
                    loadingPanel.SetActive(true); 
                    break;
                case GameState.Start: 
                    startPanel.SetActive(true); 
                    break;
                case GameState.Play: 
                    playPanel.SetActive(true); 
                    break;
                case GameState.Lose:
                    losePanel.SetActive(true);
                    break;
                case GameState.Win: 
                    winPanel.SetActive(true);
                    break;
            }
        }
    }
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance {get; protected set;}
        public Panels panels;
        [SerializeField] private Transform spawnObject;
        private List<NetworkSurvivor> survivors = new List<NetworkSurvivor>();
        private NetworkMonster monster;
        
        
        public void Awake()
        {
            Instance = this;
            panels.SetPanel(GameState.Loading);
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => spawnObject.name == "Done");
            yield return new WaitForSeconds(0.3f);
            foreach (var s in FindObjectsOfType<NetworkSurvivor>())
            {
                survivors.Add(s);
            }
            monster = FindAnyObjectByType<NetworkMonster>();
            yield return new WaitUntil(AllThePlayersReady);
            GameManager.Instance.SetOtherPlayers();
            GameManager.State = GameState.Play;
            panels.SetPanel(GameState.Play);
        }
        private bool AllThePlayersReady()
        {
            if (monster != null && !monster.IsReady()) return false; 
            for (int i = 0; i < survivors.Count; i++)
            {
                if (!survivors[i].IsReady()) return false;
            }
            return true;
        }
    }
}
