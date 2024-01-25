using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using UnityEngine;

namespace Online.Relay
{
    public class RelayManager : MonoBehaviour
    {
        private static string _relyCode;
        

        public static async Task<string> CreateRelay()
        {
            try
            { 
                var allocation = await RelayService.Instance.CreateAllocationAsync(7);
                var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                var relayServerData = new RelayServerData(allocation,"dtls");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
                NetworkManager.Singleton.StartHost();
                Debug.Log("Relay Created: " + joinCode);
                return joinCode;
            }
            catch (RelayServiceException e)
            {
                Debug.LogError(e);
                throw;
            }
        }
        public static async Task JoinRelay(string joinCode)
        {
            if(_relyCode == joinCode) return;
            _relyCode = joinCode;
            try
            {
                var jointAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                var relayServerData = new RelayServerData(jointAllocation,"dtls");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
                NetworkManager.Singleton.StartClient();
                Debug.Log("Joining Relay with " + joinCode);
            }
            catch (RelayServiceException e)
            {
                Debug.LogError(e);
                throw;
            }
        }
        public static void Logout()
        {
            NetworkManager.Singleton.Shutdown();
        }
        public static void ResetRelyCode()
        {
            _relyCode = null;
        }
    }
}
