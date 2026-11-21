using System.Collections.Generic;
using Study.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace OverFixed
{
    // 미니게임 화면. 사고 중인 패널에서 E로 열고, 사고 종류에 맞는 미니게임을 보여 주며 성공 1회를 AccidentPanel에 넘긴다
    // Q, 닫기 버튼, Esc(PauseUI)로 닫는다. 닫아도 사고, 남은 시간, 성공 횟수는 AccidentPanel에 남아 다시 열면 이어서 한다
    public class MiniGameUI : MonoBehaviour
    {
        [SerializeField] private GameObject miniGamePanel;
        [SerializeField] private Button closeButton;
        [SerializeField] private PlayerController playerController;

        [Header("미니게임 (사고 종류마다 하나)")]
        [SerializeField] private MiniGame[] miniGames;

        [Header("값 칸")]
        [SerializeField] private TMP_Text titleValue;
        [SerializeField] private TMP_Text descValue;
        [SerializeField] private TMP_Text timeValue;
        [SerializeField] private TMP_Text successValue;     // "1 / 3"

        public bool IsOpen => miniGamePanel.activeSelf;

        private AccidentPanel accidentPanel;    // 지금 화면을 연 사고 패널
        private MiniGame currentMiniGame;

        private void Start()
        {
            closeButton.onClick.AddListener(Close);
            foreach (MiniGame miniGame in miniGames) miniGame.gameObject.SetActive(false);
            miniGamePanel.SetActive(false);
        }

        private void Update()
        {
            if (IsOpen == false) return;

            if (SimpleInput.GetKeyDown(Key.Q))
            {
                Close();
                return;
            }
            DrawProgress();
        }

        public void Open(AccidentPanel targetAccidentPanel)
        {
            if (IsOpen) return;

            accidentPanel = targetAccidentPanel;
            AccidentRow accidentRow = accidentPanel.CurrentAccident;
            titleValue.text = accidentRow.Name;
            descValue.text = accidentRow.Desc;
            miniGamePanel.SetActive(true);
            playerController.LockForScreen();
            DrawProgress();

            currentMiniGame = FindMiniGame(accidentRow.AccidentID);
            currentMiniGame.Succeeded += accidentPanel.AddSuccess;
            currentMiniGame.Begin(accidentRow, accidentPanel.SuccessCount);
        }

        // 해결, 시간 초과, 정산·게임오버 화면, Q·Esc·닫기 버튼
        public void Close()
        {
            if (IsOpen == false) return;

            currentMiniGame.Succeeded -= accidentPanel.AddSuccess;
            currentMiniGame.End();
            currentMiniGame = null;
            miniGamePanel.SetActive(false);
            playerController.UnlockForScreen();
        }

        private void DrawProgress()
        {
            timeValue.text = "남은 시간 " + UIText.ToSeconds(accidentPanel.RemainingTime);
            successValue.text = accidentPanel.SuccessCount + " / " + accidentPanel.CurrentAccident.SuccessCount;
        }

        // 없으면 예외를 내서 미니게임 누락을 바로 알 수 있게 한다
        private MiniGame FindMiniGame(AccidentType accidentId)
        {
            foreach (MiniGame miniGame in miniGames)
            {
                if (miniGame.AccidentID == accidentId) return miniGame;
            }
            throw new KeyNotFoundException("미니게임 화면에 " + accidentId + " 미니게임이 없습니다.");
        }
    }
}
