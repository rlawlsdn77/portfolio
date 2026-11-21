using UnityEngine;

namespace OverFixed
{
    // 사고를 한곳에서 낸다. 항해 중 VoyageTable의 AccidentInterval 간격으로 패널 하나를 무작위로 골라 사고를 낸다
    // 사고는 동시에 하나뿐이고, 그 사고가 끝나면 다음 간격을 센다
    public class AccidentManager : MonoBehaviour
    {
        [SerializeField] private AccidentPanel[] accidentPanels;

        public AccidentPanel ActivePanel { get; private set; }  // 사고 중인 패널. 없으면 null

        private float nextAccidentTime;

        private void Start()
        {
            ScheduleNextAccident();
        }

        private void Update()
        {
            if (GameManager.Instance.IsPlaying == false) return;

            if (ActivePanel != null)
            {
                if (ActivePanel.IsAccident) return;
                ActivePanel = null;
                ScheduleNextAccident();
                return;
            }

            nextAccidentTime -= Time.deltaTime;
            if (nextAccidentTime > 0f) return;

            ActivePanel = accidentPanels[Random.Range(0, accidentPanels.Length)];
            ActivePanel.StartAccident();
        }

        private void ScheduleNextAccident()
        {
            VoyageRow voyage = GameManager.Instance.Voyage;
            nextAccidentTime = Random.Range(voyage.AccidentIntervalMin, voyage.AccidentIntervalMax);
        }
    }
}
