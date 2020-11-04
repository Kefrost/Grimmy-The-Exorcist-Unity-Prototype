using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogueBox : MonoBehaviour
{
    [SerializeField] private int lettersPerSecond;
    [SerializeField] private GameObject selectBtnBox;
    [SerializeField] private GameObject infoBtnBox;
    [SerializeField] private GameObject actionBox;
    [SerializeField] private GameObject moveBox;
    [SerializeField] private GameObject itemBox;
    [SerializeField] private GameObject moveDetails;
    [SerializeField] private Text dialogueText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Text ppText;
    [SerializeField] private Text typeText;
    [SerializeField] private Color selectedColor;

    [SerializeField] private List<Text> actionTexts;
    [SerializeField] private List<Text> moveTexts;
    [SerializeField] private List<Text> itemTexts;

    public void SetDescriptionDialogue(string message)
    {
        descriptionText.text = message;
    }

    public IEnumerator TypeDialogue(string dialogue)
    {
        dialogueText.text = "";

        foreach (var letter in dialogue)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }

        yield return new WaitForSeconds(1f);
    }

    public void TypeSingleDialogue(string dialogue)
    {
        dialogueText.text = dialogue;
    }

    public void EnableDialogueText(bool enabled)
    {
        dialogueText.enabled = enabled;
    }

    public void EnableItemOptions(bool enabled)
    {
        itemBox.SetActive(enabled);
    }
    public void EnableSelectBtnBox(bool enabled)
    {
        selectBtnBox.SetActive(enabled);
    }

    public void EnableInfoBtnBox(bool enabled)
    {
        infoBtnBox.SetActive(enabled);
    }

    public void EnableDescriptionText(bool enabled)
    {
        descriptionText.enabled = enabled;
    }

    public void EnableActionBox(bool enabled)
    {
        actionBox.SetActive(enabled);
    }

    public void EnableMoveBox(bool enabled)
    {
        moveBox.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }

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
    }

    public void UpdateItemOptionSelection(int action)
    {
        for (int i = 0; i < itemTexts.Count; i++)
        {
            if (i == action)
            {
                itemTexts[i].color = selectedColor;
            }
            else
            {
                itemTexts[i].color = Color.black;
            }
        }
    }

    public void UpdateMoveSelection(int selection, Move move)
    {
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i == selection)
            {
                moveTexts[i].color = selectedColor;
            }
            else
            {
                moveTexts[i].color = Color.black;
            }

            ppText.text = $"{move.PP}/{move.Base.PP}";
            typeText.text = move.Base.Type.ToString();

            if (move.PP == 0)
            {
                ppText.color = Color.red;
            }
            else
            {
                ppText.color = Color.black;
            }
        }
    }

    public void SetMoveNames(List<Move> moves)
    {
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i < moves.Count)
            {
                moveTexts[i].text = moves[i].Base.MoveName;
            }
            else
            {
                moveTexts[i].text = "-";
            }
        }
    }
}
