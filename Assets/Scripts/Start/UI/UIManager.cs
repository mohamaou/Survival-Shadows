using System;
using Start;
using UnityEngine;

namespace UI
{
    public enum PanelTyp 
    {
        Start, Cosmetics, Online
    }
    [Serializable]
    public class Panels
    {
        [SerializeField] private GameObject  startPanel, onlinePanel, cosmeticsPanel;

        public void SetPanel(PanelTyp panelTyp)
        {
            startPanel.SetActive(false);
            onlinePanel.SetActive(false);
            cosmeticsPanel.SetActive(false); 
            switch (panelTyp)
            {
                case PanelTyp.Start: startPanel.SetActive(true);
                    break;
                case PanelTyp.Cosmetics : cosmeticsPanel.SetActive(true);
                    break;
                case PanelTyp.Online :onlinePanel.SetActive(true);  
                    break;
            }
        }
        public void SetPanel(string panelTyp)
        {
            var p = panelTyp switch
            {
                "Start" => PanelTyp.Start,
                "Online" => PanelTyp.Online,
                "Cosmetics" => PanelTyp.Cosmetics,
                _ => PanelTyp.Start
            }; 
            SetPanel(p);
        }
    }
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance {get; private set;}
        [SerializeField] private Panels panels;
        
        
        private void Awake()
        {
            Instance = this;
            panels.SetPanel(PanelTyp.Start);
        }

        
        #region Buttons

        public void SetPanel(string panelTyp = "Start")
        {
            panels.SetPanel(panelTyp);
            if (panelTyp == "Online") GameManager.Instance.SetGameState(GameState.Options);
            if (panelTyp == "Start") GameManager.Instance.SetGameState(GameState.Start);
        }

        public void Loading()
        { 
            GameManager.Instance.SetGameState(GameState.Loading);
        }

        public void Cosmetics()
        {
            GameManager.Instance.SetGameState(GameState.Cosmetics);
        }

        #endregion
    }
}
