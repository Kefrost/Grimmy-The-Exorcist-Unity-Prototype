using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text statusText;
    [SerializeField] private HpBar hpBar;
    [SerializeField] private Color psnColor;
    [SerializeField] private Color brnColor;
    [SerializeField] private Color slpColor;
    [SerializeField] private Color parColor;
    [SerializeField] private Color frzColor;

    private Monster monster;
    private Dictionary<ConditionId, Color> statusColors;

    public void SetData(Monster monster)
    {
        this.monster = monster;

        nameText.text = monster.MBase.MonsterName;
        levelText.text = "Lvl " + monster.Level;
        hpBar.SetHp((float) monster.HP / monster.MaxHp);

        statusColors = new Dictionary<ConditionId, Color>()
        {
            { ConditionId.Psn, psnColor },
            { ConditionId.Brn, brnColor },
            { ConditionId.Slp, slpColor },
            { ConditionId.Par, parColor },
            { ConditionId.Frz, frzColor }
        };

        SetStatus();
        monster.OnStatusChanged += SetStatus;
    }

    private void SetStatus()
    {
        if (monster.Status == null)
        {
            statusText.text = String.Empty;
        }
        else
        {
            statusText.text = monster.Status.Id.ToString().ToUpper();
            statusText.color = statusColors[monster.Status.Id];
        }
        
    }

    public IEnumerator UpdateHp()
    {
        if (monster.HpChanged)
        {
            yield return hpBar.SetHpSmoothly((float)monster.HP / monster.MaxHp);
            monster.HpChanged = false;
        }
    }
}
