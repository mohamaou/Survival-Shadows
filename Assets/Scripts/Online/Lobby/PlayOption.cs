using Start;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Online.Lobby
{
    public class PlayOption : MonoBehaviour
    {
        public static PlayOption Instance { get; private set;}
        [SerializeField] private GameObject enterCodePanel;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private GameObject errorMessage; 
        
        private string _lobbyCode;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            enterCodePanel.SetActive(false);
        }
        public void SetLobbyCode()
        {
            _lobbyCode = inputField.text;
        }

        public void ShowInputPanel()
        {
            enterCodePanel.SetActive(true);
        }
        
        public void QuickPlay()
        {
            LobbyManager.Instance.QuickJoinLobby(Random.Range(0,9999).ToString(),5); 
            LobbyUIManager.Instance.SetPanel(PanelTyp.Loading); 
            GameManager.Instance.SetGameState(GameState.Loading);
        }
        public void CreatPrivateLobby()
        {
            LobbyManager.Instance.CreateLobby(Random.Range(0,9999).ToString(),5,true); 
            LobbyUIManager.Instance.SetPanel(PanelTyp.Loading); 
            GameManager.Instance.SetGameState(GameState.Loading);
        }
        public void JoinLobbyByCode()
        {
            LobbyUIManager.Instance.SetPanel(PanelTyp.Loading); 
            GameManager.Instance.SetGameState(GameState.Loading);
            LobbyManager.Instance.JoinLobbyByCode(_lobbyCode);
        }

        public void CloseInputPanel()
        {
            enterCodePanel.SetActive(false);
        }

        public void ClosePanel()
        {
            gameObject.SetActive(false);
        }

        public void NoLobbyFound()
        {
            LobbyUIManager.Instance.SetPanel(PanelTyp.SelecMap); 
            GameManager.Instance.SetGameState(GameState.Loading);
            var e =Instantiate(errorMessage, errorMessage.transform.position,Quaternion.identity,errorMessage.transform.parent);
            e.SetActive(true);
            Destroy(e,2);
        }
    }
    
}
