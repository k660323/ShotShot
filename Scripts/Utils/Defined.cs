using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defined
{
   public enum gameMode
    {
        DeathMatch, // ������ġ, �� ������ġ
        Siege,      // ������
        BattleRoyal, // ��Ʋ �ξ�
        Occupation, // ������
        Defense, // ���潺
        Raid, // ���̵�
    }

    public enum GMState
    {
        Default,
        Choice,
        GameStart,
        GameEnd,
    }
}
