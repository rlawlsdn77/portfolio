using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OverFixed
{
    // 정산 화면. 할당량을 다 내고 도착하면(GameManager.VoyageEnded) 이번 항해 기록과 도착 보상을 보여 주고,
    // 다음 버튼으로 도착 보상을 받고 정비(PLANET)로 간다
    public class SettlementUI : MonoBehaviour
    {
        [SerializeField] private GameObject settlementPanel;
        [SerializeField] private Button nextButton;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private Terminal terminal;             // 열려 있으면 먼저 닫는다
        [SerializeField] private MiniGameUI miniGameUI;         // 열려 있으면 먼저 닫는다

        [Header("값 칸")]
        [SerializeField] private TMP_Text quotaValue;
        [SerializeField] private TMP_Text earnedCreditValue;    // 이번 항해 주문 보상
        [SerializeField] private TMP_Text arrivalRewardValue;
        [SerializeField] private TMP_Text orderValue;           // 주문 성공 / 실패
        [SerializeField] private TMP_Text stabilityValue;
        [SerializeField] private TMP_Text voyageCountValue;

        // 씬을 넘어 살아남는 GameManager. 이 화면이 사라질 때 구독을 풀려고 들고 있는다
        private GameManager gameManager;

        private void Start()
        {
            gameManager = GameManager.Instance;
            gameManager.VoyageEnded += Show;
            nextButton.onClick.AddListener(gameManager.FinishSettlement);
            settlementPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            if (gameManager != null) gameManager.VoyageEnded -= Show;
        }

        private void Show()
        {
            terminal.CloseTerminal();
            miniGameUI.Close();
            playerController.LockForScreen();

            quotaValue.text = UIText.ToQuota(gameManager.PaidQuota, gameManager.Quota) + " (완납)";
            earnedCreditValue.text = "+" + UIText.ToCredit(gameManager.VoyageEarnedCredit);
            arrivalRewardValue.text = "+" + UIText.ToCredit(gameManager.Voyage.ArrivalReward);
            orderValue.text = gameManager.OrderSuccessCount + " / " + gameManager.OrderFailCount;
            stabilityValue.text = UIText.ToStability(gameManager.Stability);
            voyageCountValue.text = gameManager.VoyageIndex.ToString();
            settlementPanel.SetActive(true);
        }
    }
}
