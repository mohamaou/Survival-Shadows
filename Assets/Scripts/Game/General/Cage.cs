using DG.Tweening;
using Game.Survivors;
using UnityEngine;

namespace Game.General
{
    public class Cage : MonoBehaviour
    {
        [Header("Door")]
        [SerializeField] private Transform rightDoor;
        [SerializeField] private Transform leftDoor, rightDoorOpen, leftDoorOpen,rightDoorClose, leftDoorClose;

        [Header("Other")] 
        [SerializeField] private Transform survivorPoint;
        [SerializeField] private Transform releasePoint;
        private NetworkSurvivor mySurvivor;

        private void Start()
        {
            Open();
        }
        

        #region Door
        private void Open()
        {
            rightDoor.DORotate(rightDoorOpen.eulerAngles, 0.5f);
            leftDoor.DORotate(leftDoorOpen.eulerAngles, 0.5f);
        }
        private void Close()
        {
            rightDoor.DORotate(rightDoorClose.eulerAngles, 0.5f);
            leftDoor.DORotate(leftDoorClose.eulerAngles, 0.5f);
        }
        #endregion

        public bool IsFull() => mySurvivor != null;

        public void PurSurvivor(NetworkSurvivor survivor)
        {
            mySurvivor = survivor;
            mySurvivor.PutInCage();
            mySurvivor.transform.DOMove(survivorPoint.position,0.2f).OnComplete(Close);
            Close();
        }

        public void ReleaseSurvivor()
        {
            mySurvivor.transform.DOMove(releasePoint.position, 0.2f).OnComplete(() =>
            {
                mySurvivor.FreeFromCage();
                mySurvivor = null;
            });
            Open();
        }
    }
}
