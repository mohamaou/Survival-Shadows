using DG.Tweening;
using UnityEngine;

namespace Start
{
    public class OptionsUI : MonoBehaviour
    {
        [SerializeField] private GameObject playButton;
        [SerializeField] private OptionCharacter option1, option2;
        [SerializeField] private LayerMask option1Layer, option2Layer;
        [SerializeField] private Transform playerOption;
        private Camera _camera;
        private bool _characterMoving;

        private void Start()
        {
            _camera = Camera.main;
            playButton.SetActive(false);
            playButton.transform.localScale = Vector3.zero;
        }


        private void Update()
        {
            if(!Input.GetMouseButtonDown(0))return;
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray,Mathf.Infinity,option1Layer)) TakeOption1();
            if (Physics.Raycast(ray,Mathf.Infinity,option2Layer)) TakeOption2();
        }

        private void TakeOption1()
        {
            if (_characterMoving || playerOption.name == "Survivor") return;
            _characterMoving = true;
            option1.Move(OptionSelected);
            option2.Back();
            playerOption.name = "Survivor";
            playButton.SetActive(false);
            playButton.transform.DOScale(Vector3.one, 0.2f);
        }
        private void TakeOption2()
        {
            if (_characterMoving || playerOption.name == "Monster") return;
            _characterMoving = true;
            option2.Move(OptionSelected);
            playerOption.name = "Monster";
            option1.Back();
            playButton.SetActive(false);
            playButton.transform.DOScale(Vector3.one, 0.2f);
        }

        private void OptionSelected()
        {
            _characterMoving = false;
            playButton.SetActive(true);
        }
        public void Reset()
        {
            option1.ResetPotion();
            option2.ResetPotion();
            playerOption.name = "";
            playButton.transform.localScale = Vector3.zero;
            playButton.SetActive(false);
        }
    }
}
