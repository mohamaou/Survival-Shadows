using UnityEngine;

namespace Online.Lobby
{
  public class CloseButton : MonoBehaviour
  {
    public void Close()
    {
      LobbyManager.Instance.LeaveLobby(true);
    }
  }
}
