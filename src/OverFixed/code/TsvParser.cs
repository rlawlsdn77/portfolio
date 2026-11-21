using System;
using System.Collections.Generic;
using System.Globalization;

namespace OverFixed
{
    // TSV 파서 (열 구분은 탭). 첫 줄은 열 이름(코드가 칸을 찾는 키)이고, #으로 시작하는 한글 설명 줄은 건너뛴다
    public static class TsvParser
    {
        public static List<TRow> ParseRows<TRow>(string tsvText, Func<Dictionary<string, string>, TRow> fromCells)
        {
            List<TRow> rowList = new List<TRow>();
            string[] lines = tsvText.TrimStart('﻿').Split('\n');
            string[] header = lines[0].Trim().Split('\t');

            for (int lineIndex = 1; lineIndex < lines.Length; lineIndex++)
            {
                string line = lines[lineIndex].Trim();
                if (line.Length == 0 || line.StartsWith("#")) continue;

                string[] values = line.Split('\t');
                Dictionary<string, string> cells = new Dictionary<string, string>();
                for (int index = 0; index < header.Length; index++)
                {
                    cells[header[index].Trim()] = index < values.Length ? values[index].Trim() : "";
                }
                rowList.Add(fromCells(cells));
            }
            return rowList;
        }

        public static int ToInt(string raw)
        {
            return int.Parse(raw, CultureInfo.InvariantCulture);
        }

        public static float ToFloat(string raw)
        {
            return float.Parse(raw, CultureInfo.InvariantCulture);
        }

        // TSV ID가 enum에 없으면 여기서 예외가 나서 오타를 바로 알 수 있다
        public static T ToEnum<T>(string raw) where T : struct, Enum
        {
            return Enum.Parse<T>(raw);
        }

        // "A;B" → [A, B]. 빈 칸이면 빈 목록
        public static List<string> ToList(string raw)
        {
            List<string> list = new List<string>();
            foreach (string part in raw.Split(';'))
            {
                string value = part.Trim();
                if (value.Length > 0) list.Add(value);
            }
            return list;
        }

        // "Scrap:2|Crystal:1" → [(Scrap, 2), (Crystal, 1)]
        public static List<ItemCount> ToItemCounts(string raw)
        {
            List<ItemCount> countList = new List<ItemCount>();
            foreach (string part in raw.Split('|'))
            {
                string[] pair = part.Split(':');
                ItemCount itemCount = new ItemCount();
                itemCount.ItemID = ToEnum<ItemType>(pair[0].Trim());
                itemCount.Count = ToInt(pair[1].Trim());
                countList.Add(itemCount);
            }
            return countList;
        }
    }
}
