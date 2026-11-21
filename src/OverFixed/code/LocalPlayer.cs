using System;
using System.Collections;
using UnityEngine;

namespace OverFixed
{
    // 플레이어의 행동: E 상호작용(줍기, 제출, 터미널, 패널, 보관함, 에어락)과 Q 놓기. 입력은 PlayerController가 받는다
    public class LocalPlayer : MonoBehaviour
    {
        [Header("든 재료 (카메라 피벗에 붙여 시선을 따라간다)")]
        [SerializeField] private Transform holdParent;
        [SerializeField] private Vector3 holdOffset = new Vector3(0f, 0f, 2f);
        [SerializeField] private float holdScale = 0.5f;    // 든 동안 크기 배율 (시야를 가리지 않게)

        public ItemObject Item { get; private set; }

        public Action<float> HoldProgressChanged;   // 에어락 통과 진행도 (0 ~ 1, 0이면 끝). HoldBarUI가 구독
        public Action ItemPickedUp;                 // PlayerAnimator가 구독
        public Action DeviceUsed;                   // PlayerAnimator가 구독

        private PlayerController playerController;
        private CharacterController characterController;
        private InteractionCube interactionCube;

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            characterController = GetComponent<CharacterController>();
            interactionCube = GetComponentInChildren<InteractionCube>();
        }

        // 범위 안 가장 가까운 대상에 맞는 행동. 안내 문구는 InteractionPromptUI가 같은 규칙으로 고른다
        public void Interact()
        {
            GameObject target = interactionCube.FindNearestTarget();
            if (target == null) return;

            ItemObject item = target.GetComponent<ItemObject>();
            if (item != null)
            {
                TakeItem(item);
                return;
            }

            Terminal terminal = target.GetComponent<Terminal>();
            if (terminal != null)
            {
                UseTerminal(terminal);
                return;
            }

            AccidentPanel accidentPanel = target.GetComponent<AccidentPanel>();
            if (accidentPanel != null)
            {
                HandleAccident(accidentPanel);
                return;
            }

            Cabinet cabinet = target.GetComponent<Cabinet>();
            if (cabinet != null)
            {
                cabinet.ToggleDoors();
                return;
            }

            AirLock airLock = target.GetComponent<AirLock>();
            if (airLock != null) StartCoroutine(PassAirLock(airLock));
        }

        public void LeaveItem()
        {
            if (Item == null) return;

            Item.transform.SetParent(null);
            Item.transform.localScale = Item.OriginalScale;
            Item.transform.position = transform.position + transform.forward * holdOffset.z;
            Item.GetComponent<Collider>().enabled = true;
            // 선외에서는 놓은 자리에 떠 있게 한다
            Rigidbody itemBody = Item.GetComponent<Rigidbody>();
            if (itemBody != null) itemBody.isKinematic = playerController.MoveState == MoveState.Exterior;
            Item = null;
        }

        private void TakeItem(ItemObject item)
        {
            if (Item != null) return;

            Item = SpawnManager.Instance.TakeItem(item);
            Item.GetComponent<Collider>().enabled = false;
            Rigidbody itemBody = Item.GetComponent<Rigidbody>();
            if (itemBody != null) itemBody.isKinematic = true;
            Item.transform.SetParent(holdParent);
            Item.transform.localPosition = holdOffset;
            Item.transform.localRotation = Quaternion.identity;
            Item.transform.localScale = Item.OriginalScale * holdScale;
            ItemPickedUp?.Invoke();
        }

        // 빈손이면 터미널 화면, 재료를 들었으면 제출 (맞는 주문이 없으면 손에 남는다)
        private void UseTerminal(Terminal terminal)
        {
            if (Item == null)
            {
                DeviceUsed?.Invoke();
                terminal.OpenTerminal();
                return;
            }
            if (OrderManager.Instance.Board.Submit(Item.ItemID) == false) return;

            Destroy(Item.gameObject);
            Item = null;
            DeviceUsed?.Invoke();
        }

        private void HandleAccident(AccidentPanel accidentPanel)
        {
            if (accidentPanel.IsAccident == false) return;

            DeviceUsed?.Invoke();
            accidentPanel.OpenMiniGame();
        }

        // 통과 시간 동안 조작을 잠그고 막대를 채운 뒤 반대편으로 옮긴다
        private IEnumerator PassAirLock(AirLock airLock)
        {
            DeviceUsed?.Invoke();
            playerController.LockControl();

            float elapsedTime = 0f;
            while (elapsedTime < airLock.PassTime)
            {
                elapsedTime += Time.deltaTime;
                HoldProgressChanged?.Invoke(Mathf.Clamp01(elapsedTime / airLock.PassTime));
                yield return null;
            }

            if (playerController.MoveState == MoveState.Interior)
            {
                airLock.ToExit(characterController);
                playerController.SetMoveState(MoveState.Exterior);
            }
            else
            {
                airLock.ToEntry(characterController);
                playerController.SetMoveState(MoveState.Interior);
            }

            HoldProgressChanged?.Invoke(0f);
            playerController.UnlockControl();
        }
    }
}
