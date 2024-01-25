using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Online.Core
{
    public class Timer : NetworkBehaviour
    {
        public static Timer Instance {get; protected set;}
        [SerializeField] private float gameTime;
        [SerializeField] private TextMeshProUGUI[] timeLeftText;
        private readonly NetworkVariable<float> timeLeftNetwork = new(writePerm: NetworkVariableWritePermission.Owner);
        private bool gameStart, gameEnded;
        private float timeLeft;


        private void Awake()
        {
            Instance = this;
            timeLeft = gameTime;
            if(IsOwner)timeLeftNetwork.Value = gameTime;
        }

        private void Update()
        {
            if (IsOwner)
            {
                if (GameManager.State == GameState.Play) timeLeft -= Time.deltaTime;
                timeLeftNetwork.Value = timeLeft;
            }
            else
            {
                timeLeft = timeLeftNetwork.Value;
            }
            
            for (int i = 0; i < timeLeftText.Length; i++)
            {
                timeLeftText[i].text = TimerText();
            }
            if(timeLeft <= 0)
            {
                for (int i = 0; i < timeLeftText.Length; i++)
                {
                    timeLeftText[i].text = "00:00";
                }
                if (IsOwner)
                {
                    EndGameOnline();
                    EndGameServerRpc();
                }
            }
        }

        private string TimerText()
        {
            var minutes = Mathf.Floor(timeLeft / 60).ToString("00");
            var seconds = (timeLeft % 60).ToString("00");
            return minutes + ":" + seconds;
        }


        private void EndGameOnline()
        {
            GameManager.Instance.GameEndsOnline(EndGameTyp.TimeOut);
            timeLeft = gameTime;
        }

        [ServerRpc]
        private void EndGameServerRpc()
        { 
            EndGameClientRpc();
        }

        [ClientRpc]
        private void EndGameClientRpc()
        {
            if(IsOwner) return;
            EndGameOnline();
        }
    }
}
