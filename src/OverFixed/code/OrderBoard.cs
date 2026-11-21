using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace OverFixed
{
    // 진행 중인 주문 하나 (주문 행, 남은 시간, 재료별로 채운 개수)
    public class ActiveOrder
    {
        public OrderRow Row { get; }
        public float RemainingTime { get; private set; }

        private readonly Dictionary<ItemType, int> filledCounts = new Dictionary<ItemType, int>();

        public ActiveOrder(OrderRow row)
        {
            Row = row;
            RemainingTime = row.TimeLimit;
        }

        public bool IsComplete
        {
            get
            {
                foreach (ItemCount requireItem in Row.RequireItems)
                {
                    if (GetFilledCount(requireItem.ItemID) < requireItem.Count) return false;
                }
                return true;
            }
        }

        public int GetFilledCount(ItemType itemId)
        {
            return filledCounts.TryGetValue(itemId, out int filledCount) ? filledCount : 0;
        }

        public bool NeedsItem(ItemType itemId)
        {
            foreach (ItemCount requireItem in Row.RequireItems)
            {
                if (requireItem.ItemID == itemId && GetFilledCount(itemId) < requireItem.Count) return true;
            }
            return false;
        }

        public void Fill(ItemType itemId)
        {
            filledCounts[itemId] = GetFilledCount(itemId) + 1;
        }

        public void Tick(float deltaTime)
        {
            RemainingTime -= deltaTime;
        }
    }

    // 주문 칸 규칙. 제출한 재료는 그 재료가 필요한 주문 중 남은 시간이 가장 짧은 주문에 들어간다
    // 끝난(완료, 실패) 칸은 refillDelay 뒤 새 주문으로 채운다
    public class OrderBoard
    {
        public Action<ActiveOrder> OrderCompleted;
        public Action<ActiveOrder> OrderFailed;

        private readonly IReadOnlyList<OrderRow> candidateRows;
        private readonly float refillDelay;
        private readonly ActiveOrder[] orders;      // 빈 칸은 null
        private readonly float[] refillTimes;       // 빈 칸이 새 주문으로 채워지기까지 남은 시간

        public IReadOnlyList<ActiveOrder> Orders => orders;

        public OrderBoard(IReadOnlyList<OrderRow> candidateRows, int maxOrderCount, float refillDelay)
        {
            this.candidateRows = candidateRows;
            this.refillDelay = refillDelay;

            orders = new ActiveOrder[maxOrderCount];
            refillTimes = new float[maxOrderCount];
            for (int slotIndex = 0; slotIndex < maxOrderCount; slotIndex++)
            {
                orders[slotIndex] = CreateOrder();
            }
        }

        public float GetRefillTime(int slotIndex)
        {
            return refillTimes[slotIndex];
        }

        public void Tick(float deltaTime)
        {
            for (int slotIndex = 0; slotIndex < orders.Length; slotIndex++)
            {
                ActiveOrder order = orders[slotIndex];
                if (order == null)
                {
                    refillTimes[slotIndex] -= deltaTime;
                    if (refillTimes[slotIndex] <= 0f) orders[slotIndex] = CreateOrder();
                    continue;
                }

                order.Tick(deltaTime);
                if (order.RemainingTime <= 0f)
                {
                    EmptySlot(slotIndex);
                    OrderFailed?.Invoke(order);
                }
            }
        }

        public bool CanSubmit(ItemType itemId)
        {
            return FindTargetSlot(itemId) >= 0;
        }

        // 맞는 주문이 없으면 받지 않고 false
        public bool Submit(ItemType itemId)
        {
            int slotIndex = FindTargetSlot(itemId);
            if (slotIndex < 0) return false;

            ActiveOrder order = orders[slotIndex];
            order.Fill(itemId);
            if (order.IsComplete)
            {
                EmptySlot(slotIndex);
                OrderCompleted?.Invoke(order);
            }
            return true;
        }

        // 이 재료가 필요한 주문 중 남은 시간이 가장 짧은 칸. 없으면 -1
        private int FindTargetSlot(ItemType itemId)
        {
            int targetIndex = -1;
            for (int slotIndex = 0; slotIndex < orders.Length; slotIndex++)
            {
                ActiveOrder order = orders[slotIndex];
                if (order == null || order.NeedsItem(itemId) == false) continue;
                if (targetIndex < 0 || order.RemainingTime < orders[targetIndex].RemainingTime) targetIndex = slotIndex;
            }
            return targetIndex;
        }

        private void EmptySlot(int slotIndex)
        {
            orders[slotIndex] = null;
            refillTimes[slotIndex] = refillDelay;
        }

        private ActiveOrder CreateOrder()
        {
            return new ActiveOrder(candidateRows[Random.Range(0, candidateRows.Count)]);
        }
    }
}
