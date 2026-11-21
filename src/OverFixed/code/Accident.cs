using System;
using System.Collections.Generic;

namespace OverFixed
{
    // Unity는 enum을 숫자로 저장하므로 순서를 바꾸면 미니게임이 다른 사고를 가리킨다 (새 사고는 맨 뒤에)
    public enum AccidentType
    {
        TimingBar,
        Sequence,
        Wire,
    }

    // AccidentTable 한 줄 (패널 사고 한 종류와 그 미니게임)
    [Serializable]
    public class AccidentRow
    {
        public AccidentType AccidentID;
        public string Name;                 // 미니게임 화면 제목
        public string Desc;                 // 미니게임 화면 설명
        public float TimeLimit;             // 제한시간 (초)
        public int SuccessCount;            // 해결에 필요한 성공 횟수 (실패해도 줄지 않는다)
        public float StabilityPenalty;      // 시간 초과 시 코어 안정도 감소 (%)
        public float SpawnWeight;           // 사고 종류를 뽑는 비중

        public static AccidentRow FromCells(Dictionary<string, string> cells)
        {
            return new AccidentRow
            {
                AccidentID = TsvParser.ToEnum<AccidentType>(cells["AccidentID"]),
                Name = cells["Name"],
                Desc = cells["Desc"],
                TimeLimit = TsvParser.ToFloat(cells["TimeLimit"]),
                SuccessCount = TsvParser.ToInt(cells["SuccessCount"]),
                StabilityPenalty = TsvParser.ToFloat(cells["StabilityPenalty"]),
                SpawnWeight = TsvParser.ToFloat(cells["SpawnWeight"]),
            };
        }
    }
}
