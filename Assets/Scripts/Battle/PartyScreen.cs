using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] private Text messageText;

    private List<PartyUnitUI> unitSlots;
    private List<Monster> monsters;

    public void Init()
    {
        unitSlots = GetComponentsInChildren<PartyUnitUI>().ToList();
    }

    public void SetPartyData(List<Monster> monsters)
    {
        foreach (var slot in unitSlots)
        {
            slot.gameObject.SetActive(true);
        }

        this.monsters = monsters;

        for (int i = 0; i < unitSlots.Count; i++)
        {
            if (i < monsters.Count)
            {
                unitSlots[i].SetData(monsters[i]);
            }
            else
            {
                unitSlots[i].gameObject.SetActive(false);
            }
        }

        messageText.text = "Choose a Monster";
    }

    public void UpdateUnitSelection(int selectedUnit)
    {
        for (int i = 0; i < monsters.Count; i++)
        {
            if (i == selectedUnit)
            {
                unitSlots[i].SetSelectedUnit(true);
            }
            else
            {
                unitSlots[i].SetSelectedUnit(false);
            }
        }
    }

    public void SetMessageText(string message)
    {
        messageText.text = message;
    }
}
