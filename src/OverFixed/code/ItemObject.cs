using UnityEngine;

namespace OverFixed
{
    // 재료 오브젝트. 어떤 재료인지만 갖고, 수치는 ItemTable의 행에서 읽는다
    public class ItemObject : MonoBehaviour
    {
        [SerializeField] private ItemType itemId;

        public ItemType ItemID => itemId;
        public ItemRow Row => DataManager.GetItem(itemId);
        public Vector3 OriginalScale { get; private set; }  // 들 때 줄였다가 놓으면 되돌릴 크기

        private void Awake()
        {
            OriginalScale = transform.localScale;
        }
    }
}
