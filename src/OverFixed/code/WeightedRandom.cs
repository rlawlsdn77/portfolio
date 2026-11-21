using UnityEngine;

namespace OverFixed
{
    // 비중에 비례해 하나를 뽑는다 (재료 종류, 사고 종류)
    public static class WeightedRandom
    {
        // 비중 배열에서 하나를 골라 그 번호를 돌려준다
        // 예: {6, 3, 1.2}이면 0번이 나올 확률은 6 / 10.2
        // 비중이 0인 번호는 나오지 않는다. 모두 0이면 -1
        public static int PickIndex(float[] weights)
        {
            float total = 0f;
            for (int index = 0; index < weights.Length; index++) total += weights[index];

            float pick = Random.Range(0f, total);
            int lastIndex = -1;     // 비중이 있는 마지막 번호. Random.Range는 total도 내므로 반복문에서 못 고를 수 있다
            for (int index = 0; index < weights.Length; index++)
            {
                if (weights[index] <= 0f) continue;
                if (pick < weights[index]) return index;
                pick -= weights[index];
                lastIndex = index;
            }
            return lastIndex;
        }
    }
}
