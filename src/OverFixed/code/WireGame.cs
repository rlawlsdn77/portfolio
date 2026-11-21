using System.Collections.Generic;
using Study.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace OverFixed
{
    // 전선 잇기: 왼쪽 색 단자를 클릭한 뒤 같은 색의 오른쪽 단자를 클릭해 이으면 성공 1회
    // 쌍 수는 AccidentTable의 SuccessCount이고, 다시 열면 남은 쌍만 보인다. 오른쪽 단자는 순서를 섞는다
    public class WireGame : MiniGame
    {
        [Header("화면")]
        [SerializeField] private RectTransform leftColumn;      // 왼쪽 단자 줄 (세로 정렬)
        [SerializeField] private RectTransform rightColumn;     // 오른쪽 단자 줄 (세로 정렬)
        [SerializeField] private RectTransform wireLayer;       // 전선을 그리는 곳 (단자 뒤)
        [SerializeField] private Image terminalTemplate;        // 꺼 둔 단자 원본
        [SerializeField] private Image wireTemplate;            // 꺼 둔 전선 원본 (피벗이 왼쪽 가운데)
        [SerializeField] private Color[] wireColors;            // 단자 색. AccidentTable의 SuccessCount개 이상

        public override AccidentType AccidentID => AccidentType.Wire;

        private readonly List<Image> leftTerminals = new List<Image>();
        private readonly List<Image> rightTerminals = new List<Image>();
        private readonly List<Image> wires = new List<Image>();
        private readonly List<bool> isConnected = new List<bool>();    // 왼쪽 단자 순서대로

        private int selectedIndex = -1;     // 고른 왼쪽 단자. -1이면 고르지 않았다
        private Image selectedWire;         // 고른 단자에서 마우스를 따라가는 전선

        public override void Begin(AccidentRow accidentRow, int successCount)
        {
            base.Begin(accidentRow, successCount);
            MakeTerminals(accidentRow.SuccessCount - successCount);
        }

        public override void End()
        {
            ClearTerminals();
            base.End();
        }

        private void Update()
        {
            Vector2 pointerPosition = SimpleInput.GetMousePosition();
            if (SimpleInput.GetMouseButtonDown(SimpleInput.MouseButton.Left)) Click(pointerPosition);
            else if (selectedIndex >= 0) DrawWire(selectedWire, leftTerminals[selectedIndex].rectTransform, pointerPosition);
        }

        // 고른 단자가 없으면 왼쪽 단자를 고르고, 있으면 오른쪽 단자에 잇는다
        private void Click(Vector2 pointerPosition)
        {
            if (selectedIndex < 0) SelectTerminal(pointerPosition);
            else ConnectTerminal(pointerPosition);
        }

        // 아직 잇지 않은 왼쪽 단자를 누르면 그 색의 전선을 꺼내 마우스를 따라가게 한다
        private void SelectTerminal(Vector2 pointerPosition)
        {
            int leftIndex = FindTerminal(leftTerminals, pointerPosition);
            if (leftIndex < 0 || isConnected[leftIndex]) return;

            selectedIndex = leftIndex;
            selectedWire = Instantiate(wireTemplate, wireLayer);
            selectedWire.gameObject.SetActive(true);
            selectedWire.color = leftTerminals[leftIndex].color;
            wires.Add(selectedWire);
            DrawWire(selectedWire, leftTerminals[leftIndex].rectTransform, pointerPosition);
        }

        // 같은 색 오른쪽 단자면 연결. 다른 색이면 "색이 다릅니다", 그 밖을 누르면 고르기를 취소하고 그 자리를 다시 고른다
        private void ConnectTerminal(Vector2 pointerPosition)
        {
            int rightIndex = FindTerminal(rightTerminals, pointerPosition);
            Image leftTerminal = leftTerminals[selectedIndex];
            bool isMatch = rightIndex >= 0 && rightTerminals[rightIndex].color == leftTerminal.color;

            if (isMatch)
            {
                RectTransform rightRect = rightTerminals[rightIndex].rectTransform;
                DrawWire(selectedWire, leftTerminal.rectTransform, RectTransformUtility.WorldToScreenPoint(null, rightRect.position));
                isConnected[selectedIndex] = true;
                selectedIndex = -1;
                selectedWire = null;
                Succeed("연결");
                return;
            }

            CancelSelection();
            if (rightIndex >= 0) Miss("색이 다릅니다");
            else SelectTerminal(pointerPosition);
        }

        private void CancelSelection()
        {
            wires.Remove(selectedWire);
            Destroy(selectedWire.gameObject);
            selectedIndex = -1;
            selectedWire = null;
        }

        // 남은 쌍 수만큼 색을 골라 왼쪽은 그대로, 오른쪽은 섞어서 단자를 만든다
        private void MakeTerminals(int pairCount)
        {
            ClearTerminals();
            List<Color> colors = new List<Color>(wireColors);
            Shuffle(colors);
            colors.RemoveRange(pairCount, colors.Count - pairCount);

            foreach (Color color in colors)
            {
                leftTerminals.Add(MakeTerminal(leftColumn, color));
                isConnected.Add(false);
            }
            Shuffle(colors);
            foreach (Color color in colors) rightTerminals.Add(MakeTerminal(rightColumn, color));

            // 첫 프레임부터 단자 위치로 전선을 그릴 수 있게 정렬을 바로 맞춘다
            LayoutRebuilder.ForceRebuildLayoutImmediate(leftColumn);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rightColumn);
        }

        private Image MakeTerminal(RectTransform column, Color color)
        {
            Image terminal = Instantiate(terminalTemplate, column);
            terminal.gameObject.SetActive(true);
            terminal.color = color;
            return terminal;
        }

        private void ClearTerminals()
        {
            foreach (Image terminal in leftTerminals) Destroy(terminal.gameObject);
            foreach (Image terminal in rightTerminals) Destroy(terminal.gameObject);
            foreach (Image wire in wires) Destroy(wire.gameObject);
            leftTerminals.Clear();
            rightTerminals.Clear();
            wires.Clear();
            isConnected.Clear();
            selectedIndex = -1;
            selectedWire = null;
        }

        // 화면 좌표 위에 있는 단자의 순서. 없으면 -1 (화면 위 캔버스라 카메라는 null)
        private static int FindTerminal(List<Image> terminals, Vector2 pointerPosition)
        {
            for (int index = 0; index < terminals.Count; index++)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(terminals[index].rectTransform, pointerPosition, null)) return index;
            }
            return -1;
        }

        // 단자 가운데에서 화면 좌표까지 전선을 늘이고 돌린다
        private void DrawWire(Image wire, RectTransform from, Vector2 toScreenPosition)
        {
            Vector2 start = wireLayer.InverseTransformPoint(from.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(wireLayer, toScreenPosition, null, out Vector2 end);
            Vector2 direction = end - start;

            RectTransform wireRect = wire.rectTransform;
            wireRect.localPosition = start;
            wireRect.sizeDelta = new Vector2(direction.magnitude, wireRect.sizeDelta.y);
            wireRect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        }

        private static void Shuffle(List<Color> colors)
        {
            for (int index = colors.Count - 1; index > 0; index--)
            {
                int swapIndex = Random.Range(0, index + 1);
                Color temp = colors[index];
                colors[index] = colors[swapIndex];
                colors[swapIndex] = temp;
            }
        }
    }
}
