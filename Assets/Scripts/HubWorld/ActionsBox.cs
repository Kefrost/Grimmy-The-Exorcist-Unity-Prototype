using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionsBox : MonoBehaviour
{
    [SerializeField] private Color selectedColor;
    [SerializeField] private List<Text> actionTexts;

    //private int prevAction = -1;

    public void UpdateActionSelection(int action)
    {
        for (int i = 0; i < actionTexts.Count; i++)
        {
            if (i == action)
            {
                actionTexts[i].color = selectedColor;
            }
            else
            {
                actionTexts[i].color = Color.black;
            }
        }

        //prevAction = action;
    }
}
