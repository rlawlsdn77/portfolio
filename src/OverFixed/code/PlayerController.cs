using Study.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OverFixed
{
    public enum MoveState
    {
        Interior,   // 선내: 평면 이동, 중력
        Exterior,   // 선외: 제트팩 유영, 관성
    }

    // 1인칭 플레이어 입력, 이동, 시점. 조작 잠금과 커서도 여기서 관리한다
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerControlConfig playerControlConfig;
        [SerializeField] private MoveState startMoveState = MoveState.Interior;
        [SerializeField] private bool startJetUnlocked = true;     // 해금 상점이 없어 시연용으로 켜 둠
        [SerializeField] private Transform activityArea;           // 선외 활동 구 (반지름 = 스케일의 절반)

        public MoveState MoveState { get; private set; }
        public bool IsJetUnlocked { get; private set; }             // 선외 Shift 제트 추진
        public float VerticalLookAngle { get; private set; }        // 위아래 시선 각도 (양수가 아래). PlayerCamera가 쓴다

        // 잠금은 건 곳의 수를 세어, 겹쳐 걸어도 모두 풀어야 풀린다
        public bool IsControlLocked => controlLockCount > 0 || screenLockCount > 0;   // 이동, 상호작용
        public bool IsLookLocked => screenLockCount > 0;                              // 시점. 잠긴 동안 커서가 보인다

        private LocalPlayer localPlayer;
        private CharacterController controller;
        private float fallSpeed;        // 선내: 중력으로 쌓이는 낙하 속도
        private Vector3 spaceVelocity;  // 선외: 관성으로 유지되는 속도
        private float moveX, moveZ;     // 선내: 입력을 부드럽게 따라가는 축 값 (-1 ~ 1)
        private int controlLockCount;
        private int screenLockCount;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            localPlayer = GetComponent<LocalPlayer>();
            SetMoveState(startMoveState);
            IsJetUnlocked = startJetUnlocked;
        }

        private void Start()
        {
            UpdateCursor();
        }

        private void Update()
        {
            if (IsControlLocked == false)
            {
                if (MoveState == MoveState.Interior) UpdateInteriorMovement();
                else UpdateExteriorMovement();
            }
            if (IsLookLocked == false) UpdateLook();
            if (IsControlLocked == false)
            {
                if (SimpleInput.GetKeyDown(Key.E)) localPlayer.Interact();
                if (SimpleInput.GetKeyDown(Key.Q)) localPlayer.LeaveItem();
            }
        }

        // 에어락을 지날 때. 이전 상태의 속도는 이어가지 않는다
        public void SetMoveState(MoveState moveState)
        {
            MoveState = moveState;
            fallSpeed = 0f;
            spaceVelocity = Vector3.zero;
            moveX = 0f;
            moveZ = 0f;
        }

        // 이동·상호작용만 잠근다 (에어락 통과 중)
        public void LockControl()
        {
            controlLockCount++;
        }

        public void UnlockControl()
        {
            controlLockCount--;
        }

        // 화면(터미널, 미니게임, 일시정지, 정산, 게임오버)이 열린 동안 이동·시점·상호작용을 잠그고 커서를 보인다
        public void LockForScreen()
        {
            screenLockCount++;
            UpdateCursor();
        }

        public void UnlockForScreen()
        {
            screenLockCount--;
            UpdateCursor();
        }

        private void UpdateCursor()
        {
            Cursor.lockState = IsLookLocked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = IsLookLocked;
        }

        private void UpdateInteriorMovement()
        {
            float speed = SimpleInput.GetKey(Key.LeftShift) ? playerControlConfig.RunSpeed : playerControlConfig.MoveSpeed;
            Vector2 input = SimpleInput.GetMoveAxisRaw();
            moveX = SmoothAxis(moveX, input.x);
            moveZ = SmoothAxis(moveZ, input.y);
            Vector3 moveVector = transform.right * moveX + transform.forward * moveZ;
            controller.Move(moveVector * (speed * Time.deltaTime));

            // 상하 입력은 없고 중력으로 바닥에 붙어 있는다
            if (controller.isGrounded && fallSpeed < 0f) fallSpeed = 0f;
            fallSpeed -= playerControlConfig.Gravity * Time.deltaTime;
            controller.Move(Vector3.up * (fallSpeed * Time.deltaTime));
        }

        private void UpdateExteriorMovement()
        {
            Vector2 input = SimpleInput.GetMoveAxisRaw();
            float inputY = 0f;
            if (SimpleInput.GetKey(Key.Space)) inputY += 1f;
            if (SimpleInput.GetKey(Key.LeftCtrl)) inputY -= 1f;

            Vector3 thrust = transform.right * input.x + transform.forward * input.y + Vector3.up * inputY;
            if (thrust.sqrMagnitude > 1f) thrust.Normalize();

            // 입력은 속도에 더하고 속도는 매 프레임 줄여서, 입력을 떼도 미끄러지다 멈춘다
            spaceVelocity += thrust * (playerControlConfig.SpaceAccel * Time.deltaTime);
            spaceVelocity *= Mathf.Exp(-playerControlConfig.SpaceDrag * Time.deltaTime);

            bool isJetting = IsJetUnlocked && SimpleInput.GetKey(Key.LeftShift);
            spaceVelocity = Vector3.ClampMagnitude(spaceVelocity, isJetting ? playerControlConfig.JetSpeed : playerControlConfig.SpaceSpeed);

            PushInsideActivityArea();
            controller.Move(spaceVelocity * Time.deltaTime);
        }

        // 활동 구 밖이면 바깥으로 가는 속도를 없애고 안쪽으로 민다
        private void PushInsideActivityArea()
        {
            Vector3 fromCenter = transform.position - activityArea.position;
            if (fromCenter.magnitude <= activityArea.lossyScale.x * 0.5f) return;

            Vector3 outward = fromCenter.normalized;
            float outwardSpeed = Vector3.Dot(spaceVelocity, outward);
            if (outwardSpeed > 0f) spaceVelocity -= outward * outwardSpeed;
            spaceVelocity -= outward * (playerControlConfig.BoundaryPush * Time.deltaTime);
        }

        // 입력을 AxisSpeed로 따라간다. 반대 방향 입력이면 0부터 다시 따라간다
        private float SmoothAxis(float current, float target)
        {
            if (target != 0f && current * target < 0f) current = 0f;
            return Mathf.MoveTowards(current, target, playerControlConfig.AxisSpeed * Time.deltaTime);
        }

        private void UpdateLook()
        {
            Vector2 mouseDelta = SimpleInput.GetMouseDelta() * (playerControlConfig.MouseAxisScale * playerControlConfig.MouseSensitivity);
            transform.rotation *= Quaternion.Euler(0f, mouseDelta.x, 0f);

            // 위로는 선내·선외 한계까지, 아래로는 MaxLookDownAngle까지 (더 숙이면 1인칭에서 로봇 몸 안이 보인다)
            float maxLookAngle = MoveState == MoveState.Interior ? playerControlConfig.InteriorMaxLookAngle : playerControlConfig.ExteriorMaxLookAngle;
            float maxLookDownAngle = Mathf.Min(maxLookAngle, playerControlConfig.MaxLookDownAngle);
            VerticalLookAngle = Mathf.Clamp(VerticalLookAngle - mouseDelta.y, -maxLookAngle, maxLookDownAngle);
        }
    }
}
