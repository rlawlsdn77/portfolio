using Study.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OverFixed
{
    // 타이밍 바: 좌우로 움직이는 토글이 가운데 구간에 있을 때 Space를 누르면 성공 1회
    // 성공하면 토글이 왼쪽 끝에서 다시 출발하고, 빗나가면 TimingMissLockTime 동안 Space를 받지 않는다 (연타 방지)
    public class TimingBarGame : MiniGame
    {
        [SerializeField] private MiniGameConfig miniGameConfig;

        [Header("화면")]
        [SerializeField] private RectTransform zone;        // 가운데 성공 구간 (막대의 자식)
        [SerializeField] private RectTransform cursor;      // 움직이는 토글 (막대의 자식)

        public override AccidentType AccidentID => AccidentType.TimingBar;

        private float movedTime;        // 토글이 왼쪽 끝에서 출발한 뒤 지난 시간
        private float missLockTime;     // Space를 다시 받기까지 남은 시간

        // 토글 위치 (0 = 왼쪽 끝, 1 = 오른쪽 끝). 끝에 닿으면 되돌아온다
        private float CursorValue => Mathf.PingPong(movedTime * miniGameConfig.TimingCursorSpeed, 1f);
        private float ZoneMin => 0.5f - miniGameConfig.TimingZoneWidth * 0.5f;
        private float ZoneMax => 0.5f + miniGameConfig.TimingZoneWidth * 0.5f;

        public override void Begin(AccidentRow accidentRow, int successCount)
        {
            base.Begin(accidentRow, successCount);
            zone.anchorMin = new Vector2(ZoneMin, 0f);
            zone.anchorMax = new Vector2(ZoneMax, 1f);
            movedTime = 0f;
            missLockTime = 0f;
            DrawCursor();
        }

        private void Update()
        {
            movedTime += Time.deltaTime;
            missLockTime -= Time.deltaTime;
            DrawCursor();

            if (SimpleInput.GetKeyDown(Key.Space)) Press();
        }

        private void Press()
        {
            if (missLockTime > 0f) return;

            float cursorValue = CursorValue;
            if (cursorValue >= ZoneMin && cursorValue <= ZoneMax)
            {
                movedTime = 0f;
                Succeed("성공");
                return;
            }

            missLockTime = miniGameConfig.TimingMissLockTime;
            Miss("빗나감");
        }

        private void DrawCursor()
        {
            float cursorValue = CursorValue;
            cursor.anchorMin = new Vector2(cursorValue, cursor.anchorMin.y);
            cursor.anchorMax = new Vector2(cursorValue, cursor.anchorMax.y);
        }
    }
}
