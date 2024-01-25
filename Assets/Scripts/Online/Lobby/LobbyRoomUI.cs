using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Online.Lobby
{
    public class LobbyRoomUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI lobbyCode;
        [SerializeField] private GameObject playButton;

        private void Start()
        {
            lobbyCode.text = "Lobby Code: " + LobbyManager.Instance.GetJoinedLobby().LobbyCode;
        }

        private void Update()
        {
            playButton.SetActive(LobbyManager.Instance.IsLobbyHost() &&
                                 LobbyManager.Instance.GetJoinedLobby().Players.Count > 1);
            if(Input.GetKeyDown(KeyCode.Space))Play();
            if(LobbyManager.Instance.GetJoinedLobby().Players.Count == 5)Play();
        }

        public void Play()
        {
            if (!LobbyManager.Instance.IsLobbyHost()) return;
            NetworkManager.Singleton.SceneManager.LoadScene("Map 1", LoadSceneMode.Single);
        }
    }
}
