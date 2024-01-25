using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game.General;
using Game.Monster;
using Game.UI;
using Unity.Netcode;
using UnityEngine;

namespace Game.Survivors
{
    public enum SurvivorState
    {
        Normal, Crouched, Crawling, All, Out
    }
    public class NetworkSurvivor : NetworkBehaviour
    {
        private NetworkVariable<Vector3> _netPos = new(writePerm: NetworkVariableWritePermission.Owner);
        private NetworkVariable<float> _netRot = new(writePerm: NetworkVariableWritePermission.Owner);
        private NetworkVariable<bool> _netRun = new(writePerm: NetworkVariableWritePermission.Owner);
        private NetworkVariable<float> _netSpeed = new(writePerm: NetworkVariableWritePermission.Owner);
        private NetworkVariable<SurvivorState> _netState = new(writePerm: NetworkVariableWritePermission.Owner);
        private NetworkVariable<bool> _nerHold = new(writePerm: NetworkVariableWritePermission.Owner);
        private NetworkVariable<bool> _netInCage = new(writePerm: NetworkVariableWritePermission.Owner);
        private NetworkVariable<bool> _netReady = new(writePerm: NetworkVariableWritePermission.Owner);
        private NetworkVariable<bool> _netOut = new(writePerm: NetworkVariableWritePermission.Owner);
        private NetworkVariable<int> _netSkin = new(writePerm: NetworkVariableWritePermission.Owner);
      
        
        [SerializeField] private new SurvivorAnimation animation;
        [SerializeField] private new Collider collider;
        [SerializeField] private new Rigidbody rigidbody;
        [SerializeField] private ThirdPersonMovement movement;
        [SerializeField] private SurvivorSkin skin;
        [SerializeField] private float reviveTime = 30f, reviveRange;
        [SerializeField] private LayerMask survivorLayer,generatorLayer,cageLayer;
        [SerializeField] private Transform cam;
        private Generator.Generator myGenerator;
        private SurvivorIcon myIcon;
        private Vector3 camPoint;
        private Coroutine myRevive;
        private bool reviving;
        private Transform holdPointTransform, monsterCamera;
        public SurvivorState SurvivorState => _netState.Value;
        private bool active = true;
        private int viewIndex, survivorLeftCount, generatorsFixed;
        public bool IsReviving => reviving;
        private float timeInGenerator;
        


        private IEnumerator Start()
        {
            camPoint = cam.localPosition;
            if (IsOwner)
            {
                if (PlayerUI.Instance != null)
                {
                    PlayerUI.Instance.SetSurvivor(this);
                    StartCoroutine(Generator()); 
                    if(GameManager.Instance != null) GameManager.Instance.SetMySurvivor(this);
                }
            }

            if (!IsOwner) yield return new WaitUntil(() => _netReady.Value);
            SetSkin();
        }

        public int GetTheGeneratorFixedNumber() => generatorsFixed;
        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            { 
                _netReady.Value = true; 
                _netSkin.Value = PlayerPrefs.GetInt("Survivor Skin", 0); 
                SetSkin();
            }
        }

        public void Play()
        {
            if (IconHolder.Instance != null) myIcon = IconHolder.Instance.GetMyIcon();
        }
        private void SetSkin()
        {
            skin.SetSkin(_netSkin.Value);
        }
        

        public bool IsHold() => _nerHold.Value;
        public bool IsReady() => _netReady.Value;
        
        private void Update()
        {
            OnlineVariables();
            Keyboard();
        }

        private void LookAt(Vector3 targetPoint)
        {
            targetPoint.y = transform.position.y;
            var targetRotation = targetPoint - transform.position;
            transform.DORotate(Quaternion.LookRotation(targetRotation).eulerAngles, 0.2f);
        }
        private void Keyboard()
        {
            if(!IsOwner)return;
            if(Input.GetKeyDown(KeyCode.E))SetSurvivorState(SurvivorState.Normal);
            if(Input.GetKeyDown(KeyCode.R))SetSurvivorState(SurvivorState.Crouched);
            if(Input.GetKeyDown(KeyCode.T))SetSurvivorState(SurvivorState.Crawling);
        }

        private void OnlineVariables()
        {
            if (IsOwner)
            {
                _netPos.Value = transform.position;
                _netRot.Value = transform.eulerAngles.y;
                _netRun.Value = animation.GetMovementData().walk;
                _netSpeed.Value = animation.GetMovementData().speed;
            }
            else
            {
                if (!_nerHold.Value)
                {
                    transform.position = Vector3.Lerp(transform.position, _netPos.Value, 20 * Time.deltaTime); 
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0,_netRot.Value,0), 20*Time.deltaTime);
                } 
                if(active) animation.Movement(_netRun.Value, _netSpeed.Value);
            }
            if(myIcon == null)return;
            myIcon.Crawling(_netState.Value == SurvivorState.Crawling);
            myIcon.InPrison(_netInCage.Value);
        }
        private void LateUpdate()
        {
            if (!_nerHold.Value)
            { 
                if(IsOwner && active) cam.localPosition = camPoint;
                return;
            }
            transform.position = holdPointTransform.position;
            transform.rotation = holdPointTransform.rotation; 
            if(active && IsOwner) cam.position = monsterCamera.position;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Out") || !IsOwner) return;
            StartCoroutine(RunOut(other.transform.forward));
        }

        private IEnumerator RunOut(Vector3 direction)
        {
            SetSurvivorState(SurvivorState.Out);
            rigidbody.isKinematic = true;
            movement.active = false;
            animation.enabled = false;
            animation.Movement(true,1);
            _netOut.Value = true;
            active = false;
            PlayerUI.Instance.ShowSwitchPanel();
            cam.transform.SetParent(null);
            var otherPlayers = new List<NetworkSurvivor>();
            foreach (var survivor in FindObjectsOfType<NetworkSurvivor>())
            {
                if(survivor != this) otherPlayers.Add(survivor);
            }
            DynamicJoystick.Instance.gameObject.SetActive(false);
            while (true)
            {
                transform.position += direction * (Time.deltaTime * 2);
                var targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 6 * Time.deltaTime);
                survivorLeftCount = 0;
                for (int i = 0; i < otherPlayers.Count; i++)
                {
                    if (otherPlayers[i].GetState() == SurvivorState.Out)
                    {
                        survivorLeftCount++;
                        if (viewIndex == i) cam.position = otherPlayers[i].transform.position;
                    }
                }
                yield return null;
            }
            
        }

        public void SwitchView(int index)
        {
            viewIndex += index;
            if (viewIndex < 0) viewIndex = survivorLeftCount-1;
            if (viewIndex >= survivorLeftCount) viewIndex = 0;
        }

        #region Player State
        public SurvivorState GetState() => SurvivorState;
        public void SetSurvivorState(SurvivorState state)
        {
            animation.SetSurvivorState(state);
            if (!IsOwner) return;
            CancelRevive();
            CancelFixingTheGeneratorOnline();
            _netState.Value = state;
            SetSurvivorStateServerRpc(state);
            if (state == SurvivorState.Crawling)
            { 
                if (myRevive != null) StopCoroutine(myRevive); 
                myRevive = StartCoroutine(PlayerUI.Instance.StartReviveCountDown(reviveTime));
            }
        }
        [ServerRpc]
        private void SetSurvivorStateServerRpc(SurvivorState state)
        {
            SetSurvivorStateClientRpc(state);
        }
        [ClientRpc]
        private void SetSurvivorStateClientRpc(SurvivorState state)
        {
            if (IsOwner) return;
            if(state == SurvivorState.Normal)collider.enabled = true; 
            animation.SetSurvivorState(state);
        }
        #endregion

        #region Revive
        public bool ShowReviveButton()
        {
            if (_netState.Value != SurvivorState.Normal) return false;
            if (reviving) return false;
            if (GetNearCage() == null) return GetNearSurvivor() != null;
            if (GetNearCage().IsFull()) return true;
            return GetNearSurvivor() != null;
        }
        public void Revive()
        {
            if (GetNearCage() != null && GetNearCage().IsFull())
            {
                FreeAlly();
                return;
            }
            reviving = true;
            animation.Revive();
            myRevive = StartCoroutine(PlayerUI.Instance.StartReviveCountDown(5,GetNearSurvivor()));
            LookAt(GetNearSurvivor().transform.position);
            ReviveServerRPC();
        }

        private NetworkSurvivor GetNearSurvivor()
        {
            var result = new Collider[10];
            var size = Physics.OverlapSphereNonAlloc(transform.position, reviveRange, result, survivorLayer);
            if (size == 0) return null;
            for (int i = 0; i < result.Length; i++)
            {
                if (result[i] != null && result[i].transform != transform)
                {
                    var survivor = result[i].GetComponent<NetworkSurvivor>(); 
                    if(survivor.SurvivorState == SurvivorState.Crawling) return survivor;
                }
            }
            return null;
        }

        public void ReviveAlly()
        {
            reviving = false;
            animation.Idle();
            EndReviveServerRPC();
        }

        public void CancelRevive()
        {
            reviving = false;
            PlayerUI.Instance.HideCountdown(); 
            if(myRevive != null) StopCoroutine(myRevive);
            if (IsOwner)
            { 
                animation.Idle();
                CancelReviveServerRPC();
            }
        }
        [ServerRpc]
        private void CancelReviveServerRPC()
        {
            CancelReviveClientRpc();
        }
        [ClientRpc]
        private void CancelReviveClientRpc()
        {
            if(IsOwner)return;
            animation.Idle();
        }
        [ServerRpc]
        private void ReviveServerRPC()
        {
            ReviveClientRpc();
        }
        [ClientRpc]
        private void ReviveClientRpc()
        {
            if(IsOwner)return;
            animation.Revive();
            LookAt(GetNearSurvivor().transform.position);
        }
        [ServerRpc]
        private void EndReviveServerRPC()
        {
            EndReviveClientRpc();
        }
        [ClientRpc]
        private void EndReviveClientRpc()
        {
            if (IsOwner) return;
            animation.Idle();
            var result = new Collider[10];
            var size = Physics.OverlapSphereNonAlloc(transform.position, reviveRange, result, survivorLayer);
            if (size == 0) return;
            for (int i = 0; i < result.Length; i++)
            {
                if (result[i] != null && result[i].transform != transform)
                {
                    var survivor = result[i].GetComponent<NetworkSurvivor>();
                    if (survivor.SurvivorState == SurvivorState.Crawling && !survivor.Hold())
                    {
                        PlayerUI.Instance.HideCountdown();
                        if (survivor.myRevive != null) StopCoroutine(survivor.myRevive);
                        survivor.SetSurvivorState(SurvivorState.Normal);
                    }
                }
            }
        }
        #endregion

        #region Free Ally
        private Cage GetNearCage()
        {
            var result = new Collider[10];
            var size = Physics.OverlapSphereNonAlloc(transform.position, reviveRange, result, cageLayer);
            if (size == 0) return null;
            for (int i = 0; i < result.Length; i++)
            {
                if (result[i] != null && result[i].transform != transform)
                {
                    return result[i].GetComponent<Cage>(); 
                }
            }
            return null;
        }

        
        private void FreeAlly()
        {
            reviving = true;
            animation.FixGenerator();
            myRevive = StartCoroutine(PlayerUI.Instance.StartFreeCountDown(15));
            LookAt(GetNearCage().transform.position);
            if(IsOwner)FreeAllyServerRPC();
        }
        [ServerRpc]
        private void FreeAllyServerRPC()
        {
            FreeAllyClientRpc();
        }
        [ClientRpc]
        private void FreeAllyClientRpc()
        {
            if(IsOwner)return;
            LookAt(GetNearCage().transform.position);
            animation.FixGenerator();
        }

        public void FreeAllyEnd()
        {
            animation.Idle();
            GetNearCage().ReleaseSurvivor();
            if(IsOwner)FreeAllyEndServerRpc();
        }
        [ServerRpc]
        private void FreeAllyEndServerRpc()
        {
            FreeAllyEndClientRpc();
        }
        [ClientRpc]
        private void FreeAllyEndClientRpc()
        {
            if(IsOwner)return;
            animation.Idle();
            GetNearCage().ReleaseSurvivor();
        }
        #endregion

        #region Hold By A Monster
        public void HoldMe(Transform holdPoint)
        { 
            
            if (_nerHold.Value) return;
            if(IsOwner)_nerHold.Value = true;
            movement.active = false;
            collider.enabled = false;
            holdPointTransform = holdPoint;
            animation.GetHold(true);
            if(IsOwner)PlayerUI.Instance.HideCountdown();
            monsterCamera = MonsterAttacks.Instance.GetCam();
            if(myRevive != null) StopCoroutine(myRevive);
            if (IsOwner) HoldMeServerRpc();
        }


      
        
        [ServerRpc]
        private void HoldMeServerRpc()
        {
            HoldMeClientRpc();
        }

        [ClientRpc]
        private void HoldMeClientRpc()
        {
            if(IsOwner)return;
            collider.enabled = false;
            rigidbody.isKinematic = true;
            holdPointTransform = MonsterAttacks.Instance.GetHoldPoint();
            animation.GetHold(true);
        }


        public bool Hold() => _nerHold.Value;
        
        public void Release()
        {
            if(IsOwner) _nerHold.Value = false;
            if(IsOwner) myRevive = StartCoroutine(PlayerUI.Instance.StartReviveCountDown(reviveTime));
            rigidbody.isKinematic = false;
            movement.active = true;
            collider.enabled = true;
            animation.GetHold(false);
            if(IsOwner) ReleaseServerRpc();
        }
        [ServerRpc]
        private void ReleaseServerRpc()
        {
            ReleaseClientRpc();
        }
        [ClientRpc]
        private void ReleaseClientRpc()
        {
            if (IsOwner) return;
            movement.active = true;
            collider.enabled = true;
            rigidbody.isKinematic = false;
            animation.GetHold(false);
        }
        #endregion

        #region Generator

        private IEnumerator Generator()
        {
            while (true)
            {
                if (myGenerator != null) timeInGenerator += Time.deltaTime;
                if (myGenerator != null && movement.IsMoving() && timeInGenerator > 0.3f)
                {
                    timeInGenerator = 0;
                    CancelFixingTheGeneratorOnline();
                }
                var result = new Collider[10];
                var size = Physics.OverlapSphereNonAlloc(transform.position, reviveRange, result, generatorLayer);
                PlayerUI.Instance.ShowGeneratorButton(_netState.Value != SurvivorState.Crawling && size > 0 && !_nerHold.Value);
                yield return null;
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private Generator.Generator GetGenerator()
        {
            var result = new Collider[10];
            var size = Physics.OverlapSphereNonAlloc(transform.position, reviveRange, result, generatorLayer);
            if (size == 0) return null;
            for (int i = 0; i < result.Length; i++)
            {
                if (result[i] != null) return result[i].GetComponent<Generator.Generator>();
            }
            return null;
        }
        public void FixGeneratorOnline()
        {
            FixGenerator();
            FixGeneratorServerRpc();
        }
        [ServerRpc]
        private void FixGeneratorServerRpc()
        {
            FixGeneratorClientRpc();
        }
        [ClientRpc]
        private void FixGeneratorClientRpc()
        {
            if(IsOwner)return; 
            FixGenerator();
        }
        private void CancelFixingTheGeneratorOnline()
        {
            CancelFixingTheGenerator(false);
            CancelFixingTheGeneratorServerRpc(false);
        }
        [ServerRpc]
        private void CancelFixingTheGeneratorServerRpc(bool repaired)
        {
            CancelFixingTheGeneratorClientRpc(repaired);
        }
        [ClientRpc]
        private void CancelFixingTheGeneratorClientRpc(bool repaired)
        {
            if(IsOwner)return;
            CancelFixingTheGenerator(repaired);
        }
        
        
        private void FixGenerator()
        {
            if (IsOwner) Game.Generator.GeneratorMiniGame.Instance.StartRepair(GeneratorRepaired);
            animation.FixGenerator();
            myGenerator = GetGenerator();
            myGenerator.InUse();
            var generatorPos = myGenerator.transform.position;
            generatorPos.y = transform.position.y;
            transform.LookAt(generatorPos);
        }
        private void CancelFixingTheGenerator(bool repaired)
        {
            if (myGenerator != null)
            { 
                if (repaired) 
                { 
                    myGenerator.Repaired(); 
                }
                else
                { 
                    myGenerator.CancelUse(); 
                }
                myGenerator = null;
            }
            animation.Idle();
            if(IsOwner && !repaired && Game.Generator.GeneratorMiniGame.Instance != null)Game.Generator.GeneratorMiniGame.Instance.Cancel();
        }

        private void GeneratorRepaired()
        {
            generatorsFixed ++;
            animation.Idle();
            myGenerator.Repaired(); 
            myGenerator = null;
            CancelFixingTheGeneratorServerRpc(true);
        }
        #endregion

        #region Cage

        public bool IsInCage() => _netInCage.Value;
        public void PutInCage()
        {
            if (IsOwner)
            {
                _netInCage.Value = true;
                movement.active = false; 
                collider.enabled = false; 
                rigidbody.isKinematic = true; 
                _nerHold.Value = false;
            }
            animation.PutInCage();
        }
        public void FreeFromCage()
        {
            if (IsOwner)
            {
                _netInCage.Value = false;
                movement.active = true; 
                collider.enabled = true; 
                rigidbody.isKinematic = false;
                _nerHold.Value = false;
            }
            animation.Idle();
            animation.GetHold(false);
            SetSurvivorState(SurvivorState.Normal);
        }
        #endregion

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position,reviveRange);
        }
        
    }
}
