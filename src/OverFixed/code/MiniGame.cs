using System;
using TMPro;
using UnityEngine;

namespace OverFixed
{
    // 사고 미니게임 한 종류 (MiniGameUI의 내용 영역에 종류마다 하나)
    // 화면이 열리면 Begin, 닫히면 End. 성공 1회마다 Succeed를 부르고, 해결 판정과 제한시간은 AccidentPanel이 맡는다
    public abstract class MiniGame : MonoBehaviour
    {
        [Header("결과 글자")]
        [SerializeField] private TMP_Text resultValue;
        [SerializeField] private Color successColor = Color.cyan;
        [SerializeField] private Color missColor = Color.red;

        public abstract AccidentType AccidentID { get; }

        public Action Succeeded;    // MiniGameUI가 구독해 AccidentPanel에 넘긴다

        // successCount는 이번 사고에서 이미 모은 성공 횟수 (닫았다 다시 열면 이어서 한다)
        public virtual void Begin(AccidentRow accidentRow, int successCount)
        {
            resultValue.text = "";
            gameObject.SetActive(true);
        }

        public virtual void End()
        {
            gameObject.SetActive(false);
        }

        protected void Succeed(string resultText)
        {
            ShowResult(resultText, successColor);
            Succeeded?.Invoke();
        }

        protected void Miss(string resultText)
        {
            ShowResult(resultText, missColor);
        }

        private void ShowResult(string text, Color color)
        {
            resultValue.text = text;
            resultValue.color = color;
        }
    }
}
