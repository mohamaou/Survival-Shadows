using System.Collections;
using Game.General;
using Game.Survivors;
using Game.UI;
using Unity.Netcode;
using UnityEngine;


namespace Game.Monster
{
    public class NetworkMonster : NetworkBehaviour
    {
        private NetworkVariable<Vector3> _netPos = new(writePerm: NetworkVariableWritePermission.Owner);
        private NetworkVariable<float> _netRot = new(writePerm: NetworkVariableWritePermission.Owner);
        private NetworkVariable<Vector2> _netAnimeDirection = new(writePerm: NetworkVariableWritePermission.Owner);
        private NetworkVariable<bool> _netWalkAnimation = new(writePerm: NetworkVariableWritePermission.Owner);
        private NetworkVariable<bool> _netHold = new(writePerm: NetworkVariableWritePermission.Owner);
        private NetworkVariable<bool> _netReady = new(writePerm: NetworkVariableWritePermission.Owner);
        private NetworkVariable<int> _netSkin = new(writePerm: NetworkVariableWritePermission.Owner);
        
        
        
        
        [SerializeField] private FirstPersonMovement movement;
        [SerializeField] private FirstPersonCameraMovement cameraMovement;
        [SerializeField] private new MonsterAnimationManager animation;
        [SerializeField] private MonsterAttacks attacks;
        [SerializeField] private new GameObject camera;
        [SerializeField] private GameObject thirdPerson, firstPerson;
        [SerializeField] private LayerMask cageLayer;
        [SerializeField] private MonsterSkin skin;

        private NetworkSurvivor survivorHolding;
        private bool attacking;
          
        
        private void Start()
        {
            thirdPerson.SetActive(!IsOwner);
            firstPerson.SetActive(IsOwner);
            if (IsOwner)
            { 
                if(PlayerUI.Instance != null) PlayerUI.Instance.SetMonster(this);
                if (GameManager.Instance != null) GameManager.Instance.SetMyMonster(this);
                skin.SetSkin(PlayerPrefs.GetInt("Monster Skin", 0));
            }
            else
            {
                movement.enabled = false; 
                cameraMovement.enabled = false; 
                Destroy(camera);
            }
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                _netSkin.Value = PlayerPrefs.GetInt("Monster Skin", 0);
                _netReady.Value = true;
            }
            else
            {
                StartCoroutine(PlayerReady());
            }
        }

        private IEnumerator PlayerReady()
        {
            yield return new WaitUntil(() => _netReady.Value);
            skin.SetSkin(_netSkin.Value);
        }

        public bool IsReady() => _netReady.Value;
        
        
        private void Update()
        {
            OnlineVariables();
        }
        private void OnlineVariables()
        {
            if (IsOwner)
            {
                _netPos.Value = transform.position;
                _netRot.Value = thirdPerson.transform.eulerAngles.y;
                _netAnimeDirection.Value = animation.GetDirection();
                _netWalkAnimation.Value = animation.IsWalking();
                _netHold.Value = animation.IsHolding();
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, _netPos.Value, 20 * Time.deltaTime);
                thirdPerson.transform.rotation = Quaternion.Lerp( thirdPerson.transform.rotation, Quaternion.Euler(0,_netRot.Value,0), 20*Time.deltaTime);
                animation.SetMoveDirection(_netAnimeDirection.Value);
                animation.SetWalk(_netWalkAnimation.Value);
                animation.Hold(_netHold.Value);
            }
        }
        

        #region Online Events
        public void Attack()
        {
            if (attacking) return;
            attacking = true;
            movement.enabled = false;
            animation.Attack();
            AttackServerRpc();
        }
        [ServerRpc]
        private void AttackServerRpc()
        {
            AttackClientRpc();
        }
        [ClientRpc] 
        private void AttackClientRpc()
        {
            if (IsOwner) return;
            animation.Attack();
        }
        public void AttackEnd()
        {
            movement.enabled = true;
            attacking = false;
        }
        #endregion

        #region Buttons Visibility
        public bool ShowHoldButton() => attacks.ShowHoldButton() && !ShowReleaseButton();

        public bool ShowReleaseButton()
        {
            if (survivorHolding == null) return false;
            return survivorHolding.IsHold();
        }

        public bool ShowCageButton()
        {
            if (survivorHolding == null) return false;
            var cage = GetNearCage();
            if (cage == null) return false;
            return !cage.IsFull();
        }

        #endregion

        #region Holding Player
        //Hold
        public void HoldSurvivor()
        {
            if (ShowReleaseButton()) return;
            survivorHolding = attacks.GetNearSurvivor(SurvivorState.Crawling);
            animation.Hold(true);
            HoldSurvivorServerRpc();
        }
        [ServerRpc]
        private void HoldSurvivorServerRpc()
        {
            HoldSurvivorClientRpc();
        }
        [ClientRpc] 
        private void HoldSurvivorClientRpc()
        {
            if (IsOwner) return;
            survivorHolding = attacks.GetNearSurvivor(SurvivorState.Crawling);
            animation.Hold();
        }
        
        //Release
        public void ReleaseSurvivor()
        {
            if (IsOwner)
            {
                survivorHolding = null;
                animation.Hold(false);
            }
            ReleaseSurvivorServerRpc();
        } 
        [ServerRpc]
        private void ReleaseSurvivorServerRpc()
        {
            ReleaseSurvivorClientRpc();
        }
        [ClientRpc]
        private void ReleaseSurvivorClientRpc()
        {
            if (IsOwner) return;
            survivorHolding.Release();
            survivorHolding = null;
        }
        
        //Cage
        private Cage GetNearCage()
        {
            var results = new Collider[10];
            var size = Physics.OverlapSphereNonAlloc(transform.position, attacks.GetRange(), results, cageLayer);
            if (size == 0) return null;
            for (int i = 0; i < results.Length; i++)
            {
                if (results[i] != null) return results[i].GetComponent<Cage>();
            }
            return null;
        }

        public void PutSurvivorInCage()
        {
            animation.Hold(false);
            GetNearCage().PurSurvivor(survivorHolding);
            survivorHolding = null;
            PutSurvivorInCageServerRpc();
        }
        [ServerRpc]
        private void PutSurvivorInCageServerRpc()
        {
            PutSurvivorInCageClientRpc();
        }
        [ClientRpc]
        private void PutSurvivorInCageClientRpc()
        {
            if (IsOwner) return;
            GetNearCage().PurSurvivor(survivorHolding);
            survivorHolding = null;
        }

        #endregion
    }
}
