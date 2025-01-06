using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defined
{
   public enum gameMode
    {
        DeathMatch, // 데스매치, 팀 데스매치
        Siege,      // 공성전
        BattleRoyal, // 배틀 로얄
        Occupation, // 점령전
        Defense, // 디펜스
        Raid, // 레이드
    }

    public enum GMState
    {
        Default,
        Choice,
        GameStart,
        GameEnd,
    }
}
