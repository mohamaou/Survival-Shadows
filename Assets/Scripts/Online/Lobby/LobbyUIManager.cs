using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Online.Lobby
{
    public enum PanelTyp 
    {
        Start, Loading, Find, Lobby, SelecMap
    }
    [Serializable]
    public class Panels
    {
        [SerializeField] private GameObject  startPanel, mapPanel, loadingPanel, lobbyPanel;

        public void SetPanel(PanelTyp panelTyp)
        {
            startPanel.SetActive(false);
            lobbyPanel.SetActive(false);
            loadingPanel.SetActive(false);
            mapPanel.SetActive(false);
            switch (panelTyp)
            {
                case PanelTyp.Start: startPanel.SetActive(true);
                    break;
                case PanelTyp.Lobby :  lobbyPanel.SetActive(true);
                    break;
                case PanelTyp.SelecMap: mapPanel.SetActive(true);
                    break;
                case PanelTyp.Loading : loadingPanel.SetActive(true); 
                    break;
            }
        }
    }
    
    public class LobbyUIManager : MonoBehaviour
    {
        public static LobbyUIManager Instance {get; private set;}
        [SerializeField] private Panels panels; 
        private Transform playerOption;


        private void Awake()
        {
            Instance = this;
            playerOption = GameObject.FindWithTag("PlayerOption").transform;
        }

        private void Start()
        {
            panels.SetPanel(PanelTyp.Start);
        }

        public void SetPanel(string panelTyp)
        {
            var p = panelTyp switch
            {
                "Start" => PanelTyp.Start,
                "Find" => PanelTyp.Find,
                "Lobby" => PanelTyp.Lobby,
                "Loading" => PanelTyp.Loading,
                "Map" => PanelTyp.SelecMap,
                _ => PanelTyp.Start
            };
            print(p);
            panels.SetPanel(p);
        }
        public void SetPanel(PanelTyp p)
        {
            panels.SetPanel(p);
        }

        public void ResetOnlinePanel()
        {
            
        }
        

        private void Update()
        {
            var p = playerOption.name switch
            {
                "Monster" => PlayerTyp.Monster,
                _ => PlayerTyp.Survivor
            };
            LobbyManager.Instance.SetPlayerTyp(p);
        }

        #region Buttons
        public void QuickJoinLobby()
        { 
            LobbyManager.Instance.QuickJoinLobby(Random.Range(0, 999).ToString(), LobbyManager.Instance.GetMaxPlayerCount());
            panels.SetPanel(PanelTyp.Loading);
        }

        public void LeaveLobby()
        {
            LobbyManager.Instance.LeaveLobby();
        }
        
        public void Play()
        { 
            LobbyManager.Instance.Play();
        }
        #endregion
    }
}
