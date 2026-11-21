using System.Collections.Generic;
using UnityEngine;

namespace OverFixed
{
    // 밸런스 테이블(TSV) 창구. 처음 부를 때 Resources/Tsv에서 읽는다
    public static class DataManager
    {
        private const string TsvFolder = "Tsv/";

        private static Dictionary<ItemType, ItemRow> itemTable;
        private static List<OrderRow> orderTable;
        private static List<VoyageRow> voyageTable;
        private static List<AccidentRow> accidentTable;

        // 테이블에 없는 재료면 예외가 나서 TSV 누락을 바로 알 수 있다
        public static ItemRow GetItem(ItemType itemId)
        {
            if (itemTable == null)
            {
                itemTable = new Dictionary<ItemType, ItemRow>();
                foreach (ItemRow itemRow in TsvParser.ParseRows(LoadTsv("ItemTable"), ItemRow.FromCells))
                {
                    itemTable[itemRow.ItemID] = itemRow;
                }
            }
            return itemTable[itemId];
        }

        public static IReadOnlyList<OrderRow> GetOrderRows()
        {
            if (orderTable == null) orderTable = TsvParser.ParseRows(LoadTsv("OrderTable"), OrderRow.FromCells);
            return orderTable;
        }

        // 항해 번호(1부터)의 행. 표의 마지막 항해보다 뒤면 마지막 행
        public static VoyageRow GetVoyage(int voyageIndex)
        {
            if (voyageTable == null) voyageTable = TsvParser.ParseRows(LoadTsv("VoyageTable"), VoyageRow.FromCells);
            int rowIndex = Mathf.Clamp(voyageIndex, 1, voyageTable.Count) - 1;
            return voyageTable[rowIndex];
        }

        public static IReadOnlyList<AccidentRow> GetAccidentRows()
        {
            if (accidentTable == null) accidentTable = TsvParser.ParseRows(LoadTsv("AccidentTable"), AccidentRow.FromCells);
            return accidentTable;
        }

        private static string LoadTsv(string tableName)
        {
            TextAsset tsvAsset = Resources.Load<TextAsset>(TsvFolder + tableName);
            if (tsvAsset == null) throw new KeyNotFoundException("Resources/" + TsvFolder + tableName + ".tsv 파일이 없습니다.");
            return tsvAsset.text;
        }
    }
}
