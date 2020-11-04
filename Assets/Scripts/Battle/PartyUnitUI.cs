using UnityEngine;
using UnityEngine.UI;

public class PartyUnitUI : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text levelText;
    [SerializeField] private HpBar hpBar;
    [SerializeField] private Color selectedColor;

    //private Monster monster;

    public void SetData(Monster monster)
    {
        //this.monster = monster;

        nameText.text = monster.MBase.MonsterName;

        levelText.text = "Lvl " + monster.Level;

        if (hpBar != null)
        {
            hpBar.SetHp((float)monster.HP / monster.MaxHp);
        }
    }

    public void SetSelectedUnit(bool selected)
    {
        if (selected)
        {
            nameText.color = selectedColor;
        }
        else
        {
            nameText.color = Color.black;
        }
    }
}
