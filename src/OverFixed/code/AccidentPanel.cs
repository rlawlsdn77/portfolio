using System.Collections.Generic;
using UnityEngine;

namespace OverFixed
{
    // 사고가 나는 패널. 사고를 낼 때와 패널은 AccidentManager가 고르고, 종류는 AccidentTable의 SpawnWeight로 뽑는다
    // E로 연 미니게임에서 SuccessCount만큼 성공하면 해결, 제한시간이 지나면 안정도를 StabilityPenalty만큼 깎고 끝난다
    public class AccidentPanel : MonoBehaviour
    {
        [SerializeField] private MiniGameUI miniGameUI;
        [SerializeField] private string locationName;   // HUD 사고 알림에 붙는 자리 이름 ("중앙 모듈" 등)

        public AccidentRow CurrentAccident { get; private set; }  // 사고가 없으면 null
        public bool IsAccident => CurrentAccident != null;
        public float RemainingTime { get; private set; }
        public int SuccessCount { get; private set; }             // 화면을 닫았다 다시 열어도 남는다
        public string LocationName => locationName;

        private void Update()
        {
            if (GameManager.Instance.IsPlaying == false) return;
            if (IsAccident == false) return;

            RemainingTime -= Time.deltaTime;
            if (RemainingTime <= 0f) FailAccident();
        }

        public void OpenMiniGame()
        {
            miniGameUI.Open(this);
        }

        // 미니게임 성공 1회 (MiniGameUI)
        public void AddSuccess()
        {
            if (IsAccident == false) return;

            SuccessCount++;
            if (SuccessCount >= CurrentAccident.SuccessCount) EndAccident();
        }

        // AccidentManager가 부른다
        public void StartAccident()
        {
            IReadOnlyList<AccidentRow> accidentRows = DataManager.GetAccidentRows();
            float[] weights = new float[accidentRows.Count];
            for (int index = 0; index < accidentRows.Count; index++) weights[index] = accidentRows[index].SpawnWeight;
            CurrentAccident = accidentRows[WeightedRandom.PickIndex(weights)];
            RemainingTime = CurrentAccident.TimeLimit;
            SuccessCount = 0;
        }

        private void FailAccident()
        {
            GameManager.Instance.ChangeStability(-CurrentAccident.StabilityPenalty);
            EndAccident();
        }

        private void EndAccident()
        {
            CurrentAccident = null;
            miniGameUI.Close();
        }
    }
}
