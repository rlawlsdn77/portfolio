using System;
using Jay;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OverFixed
{
    public enum GameOverReason
    {
        StabilityZero,  // 코어 안정도 0%
        QuotaUnpaid,    // 도착할 때 할당량 미납
    }

    // 씬을 넘어 이어지는 게임 값(크레딧, 안정도, 항해 번호, 할당량)과 그 규칙. 값은 이 클래스의 함수로만 바뀐다
    // 항해 씬이 열리면 항해를 시작하고, 항해 시간이 지나면 정산(할당량 완납) 또는 게임오버(미납)
    public class GameManager : SingletonBase<GameManager>
    {
        [SerializeField] private CreditConfig creditConfig;
        [SerializeField] private CoreConfig coreConfig;

        [Header("씬 이름")]
        [SerializeField] private string voyageSceneName = "SPACE";
        [SerializeField] private string titleSceneName = "MainMenu";
        [SerializeField] private string planetSceneName = "PLANET";

        public int Credit { get; private set; }
        public float Stability { get; private set; }        // 코어 안정도 (%)
        public int VoyageIndex { get; private set; }        // 지금 항해 번호 (1부터)
        public int TotalEarnedCredit { get; private set; }  // 게임 전체에서 번 크레딧 (주문 보상 + 도착 보상)

        // 이번 항해 기록 (정산 화면)
        public int VoyageEarnedCredit { get; private set; }
        public int OrderSuccessCount { get; private set; }
        public int OrderFailCount { get; private set; }
        public int PaidQuota { get; private set; }

        public bool IsPlaying { get; private set; }
        public float RemainingTime { get; private set; }    // 도착까지 남은 시간 (초)

        public RecordSave Record { get; private set; }
        public bool IsNewRecord { get; private set; }
        public GameOverReason LastGameOverReason { get; private set; }

        public VoyageRow Voyage => DataManager.GetVoyage(VoyageIndex);
        public VoyageRow NextVoyage => DataManager.GetVoyage(VoyageIndex + 1);
        public int CompletedVoyageCount => VoyageIndex - 1;     // 게임오버 때 지금 항해는 완료로 세지 않는다

        public int Quota => Voyage.QuotaBase;
        public int RemainingQuota => Quota - PaidQuota;
        public bool IsQuotaPaid => PaidQuota >= Quota;
        public int MaxQuotaPayment => Mathf.Min(Credit, RemainingQuota);

        // 화면 표기(올림)가 상한이면 가득 찬 것으로 본다 (99.98%일 때 사서 크레딧만 잃지 않게)
        public bool IsStabilityFull => Mathf.CeilToInt(Stability) >= coreConfig.MaxStability;
        public bool CanBuyStabilizer => Credit >= coreConfig.StabilizerPrice && IsStabilityFull == false;

        public Action VoyageEnded;          // 정산 화면이 구독
        public Action GameOverHappened;     // 게임오버 화면이 구독

        // 다른 스크립트가 Awake보다 먼저 Instance를 부르면 SingletonBase가 DontDestroyOnLoad를 건너뛰므로 한 번 더 건다
        protected override void Awake()
        {
            base.Awake();
            if (instance == this) DontDestroyOnLoad(gameObject);
        }

        protected override void OnInitialize()
        {
            Record = RecordSave.Load();
            ResetGame();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        protected override void OnDispose()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == voyageSceneName) StartVoyage();
        }

        private void Update()
        {
            if (IsPlaying == false) return;

            // 안정도는 초당 BaseDecay씩 줄고, 0%가 되면 ChangeStability가 게임오버를 낸다
            ChangeStability(-Voyage.BaseDecay * Time.deltaTime);
            if (IsPlaying == false) return;

            RemainingTime -= Time.deltaTime;
            if (RemainingTime <= 0f) Arrive();
        }

        private void ResetGame()
        {
            Credit = creditConfig.StartCredit;
            Stability = coreConfig.StartStability;
            VoyageIndex = 1;
            TotalEarnedCredit = 0;
        }

        private void StartVoyage()
        {
            VoyageEarnedCredit = 0;
            OrderSuccessCount = 0;
            OrderFailCount = 0;
            PaidQuota = 0;
            RemainingTime = Voyage.VoyageTime;
            IsPlaying = true;
        }

        // 도착. 할당량을 다 냈으면 정산, 못 냈으면 게임오버
        private void Arrive()
        {
            RemainingTime = 0f;
            if (IsQuotaPaid == false)
            {
                GameOver(GameOverReason.QuotaUnpaid);
                return;
            }
            IsPlaying = false;
            VoyageEnded?.Invoke();
        }

        // 0 ~ MaxStability로 자르고, 항해 중 0%가 되면 게임오버
        public void ChangeStability(float amount)
        {
            Stability = Mathf.Clamp(Stability + amount, 0f, coreConfig.MaxStability);
            if (IsPlaying && Stability <= 0f) GameOver(GameOverReason.StabilityZero);
        }

        public void CompleteOrder(int reward)
        {
            Credit += reward;
            VoyageEarnedCredit += reward;
            TotalEarnedCredit += reward;
            OrderSuccessCount++;
            ChangeStability(coreConfig.OrderSuccessRecovery);
        }

        public void FailOrder()
        {
            OrderFailCount++;
            ChangeStability(-coreConfig.OrderFailPenalty);
        }

        // 안정화 물질 구매 (터미널, PLANET 정비). 횟수 제한 없음
        public void BuyStabilizer()
        {
            if (CanBuyStabilizer == false) return;

            Credit -= coreConfig.StabilizerPrice;
            ChangeStability(coreConfig.StabilizerRecovery);
        }

        // 할당량 납부 (터미널). 항해 중에 1 ~ MaxQuotaPayment만 낼 수 있다
        public void PayQuota(int amount)
        {
            if (IsPlaying == false || amount <= 0 || amount > MaxQuotaPayment) return;

            Credit -= amount;
            PaidQuota += amount;
        }

        private void GameOver(GameOverReason reason)
        {
            IsPlaying = false;
            LastGameOverReason = reason;
            IsNewRecord = Record.TryUpdate(CompletedVoyageCount, TotalEarnedCredit);
            GameOverHappened?.Invoke();
        }

        // 정산 화면의 다음 버튼. 도착 보상은 누적 크레딧에만 넣고 이번 항해에 번 크레딧에는 넣지 않는다
        public void FinishSettlement()
        {
            Credit += Voyage.ArrivalReward;
            TotalEarnedCredit += Voyage.ArrivalReward;
            SceneManager.LoadScene(planetSceneName);
        }

        // PLANET의 다음 항해 시작
        public void StartNextVoyage()
        {
            VoyageIndex++;
            SceneManager.LoadScene(voyageSceneName);
        }

        // 타이틀의 출항 시작, 게임오버의 다시 시작
        public void StartGame()
        {
            ResetGame();
            SceneManager.LoadScene(voyageSceneName);
        }

        // 일시정지의 항해 포기. 게임오버처럼 최고 기록을 갱신한다
        public void AbandonVoyage()
        {
            Record.TryUpdate(CompletedVoyageCount, TotalEarnedCredit);
            GoToTitle();
        }

        public void GoToTitle()
        {
            IsPlaying = false;
            SceneManager.LoadScene(titleSceneName);
        }
    }
}
