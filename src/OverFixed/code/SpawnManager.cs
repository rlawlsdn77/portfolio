using System.Collections.Generic;
using DG.Tweening;
using Jay;
using UnityEngine;

namespace OverFixed
{
    // 선외 재료 생성. 재료는 spawnSphere 위쪽 반구 표면에서 생겨, safeSphere(함선)를 위로 스치는 접선 방향으로
    // 반대편 표면까지 흐른 뒤 풀로 돌아간다 (에어락이 지붕에 있어 함선 위로 지나가야 잡기 쉽다)
    public class SpawnManager : SingletonBase<SpawnManager>
    {
        [SerializeField] private SpawnConfig spawnConfig;
        [SerializeField] private ItemObject[] itemPrefabs;  // 생성할 재료 종류. 비중은 ItemTable의 SpawnWeight
        [SerializeField] private Transform spawnSphere;     // 반지름 = 스케일의 절반
        [SerializeField] private Transform safeSphere;

        private readonly List<ItemObject> pooledItems = new List<ItemObject>();
        private float spawnTimer;   // 마지막 생성 뒤 지난 시간

        private float SpawnRadius => spawnSphere.lossyScale.x * 0.5f;
        private float SafeRadius => safeSphere.lossyScale.x * 0.5f;

        protected override void Awake()
        {
            base.Awake();

            // 종류마다 PoolCount개를 꺼 둔 채 만들어 둔다
            foreach (ItemObject itemPrefab in itemPrefabs)
            {
                for (int count = 0; count < spawnConfig.PoolCount; count++)
                {
                    ItemObject item = Instantiate(itemPrefab, transform);
                    item.GetComponent<Rigidbody>().isKinematic = true;
                    item.transform.localScale = Vector3.one * spawnConfig.FlowItemScale;
                    item.gameObject.SetActive(false);
                    pooledItems.Add(item);
                }
            }
        }

        // 항해 중에만 초당 BaseSpawnRate개씩 만든다. 넘친 시간은 남겨 개수를 맞춘다
        private void Update()
        {
            if (GameManager.Instance.IsPlaying == false) return;

            spawnTimer += Time.deltaTime;
            float spawnInterval = 1f / spawnConfig.BaseSpawnRate;
            if (spawnTimer < spawnInterval) return;
            spawnTimer -= spawnInterval;

            float[] weights = new float[itemPrefabs.Length];
            for (int index = 0; index < itemPrefabs.Length; index++) weights[index] = itemPrefabs[index].Row.SpawnWeight;
            ItemObject itemPrefab = itemPrefabs[WeightedRandom.PickIndex(weights)];
            ItemObject item = GetInactiveItem(itemPrefab.ItemID);
            if (item != null) Spawn(item);
        }

        // 잡은 재료. 흐르는 풀 재료는 풀로 돌려보내고 같은 종류의 새 인스턴스를 준다 (풀이 줄지 않아 생성 비율이 유지된다)
        // 프리팹에서 만들어야 OriginalScale이 풀 크기로 잡히지 않는다. 놓아 둔 재료는 그대로 준다
        public ItemObject TakeItem(ItemObject item)
        {
            if (pooledItems.Contains(item) == false) return item;

            ReturnItem(item);
            return Instantiate(GetPrefab(item.ItemID));
        }

        private void Spawn(ItemObject item)
        {
            Vector3 spawnDirection = Random.onUnitSphere;
            spawnDirection.y = Mathf.Abs(spawnDirection.y);
            Vector3 startPosition = spawnSphere.position + spawnDirection * SpawnRadius;

            // 함선 중심 방향에서 접선 각도만큼 위로 꺾는다 (sin(접선 각도) = 함선 반지름 / 중심까지 거리)
            Vector3 toSafeCenter = safeSphere.position - startPosition;
            float tangentAngle = Mathf.Asin(SafeRadius / toSafeCenter.magnitude);
            Vector3 upward = Vector3.ProjectOnPlane(Vector3.up, toSafeCenter).normalized;
            Vector3 moveDirection = Mathf.Cos(tangentAngle) * toSafeCenter.normalized + Mathf.Sin(tangentAngle) * upward;

            // 구 표면에서 출발해 반대편 표면까지 가는 거리
            float moveDistance = -2f * Vector3.Dot(startPosition - spawnSphere.position, moveDirection);
            Vector3 endPosition = startPosition + moveDirection * moveDistance;

            item.transform.position = startPosition;
            item.transform.rotation = Quaternion.identity;
            item.gameObject.SetActive(true);
            item.transform.DOMove(endPosition, moveDistance / spawnConfig.FlowSpeed).SetEase(Ease.Linear).OnComplete(() => ReturnItem(item));
        }

        private void ReturnItem(ItemObject item)
        {
            item.transform.DOKill();
            item.gameObject.SetActive(false);
        }

        private ItemObject GetInactiveItem(ItemType itemId)
        {
            foreach (ItemObject item in pooledItems)
            {
                if (item.ItemID == itemId && item.gameObject.activeSelf == false) return item;
            }
            return null;
        }

        private ItemObject GetPrefab(ItemType itemId)
        {
            foreach (ItemObject itemPrefab in itemPrefabs)
            {
                if (itemPrefab.ItemID == itemId) return itemPrefab;
            }
            return null;
        }
    }
}
