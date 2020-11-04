using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMenuBox : MonoBehaviour
{
    [SerializeField] private Text messageText;
    [SerializeField] private Color selectedColor;
    [SerializeField] private List<Text> options;

    private int currentAction;
    private bool isWon;

    public void HandleUpdate()
    {
        InputManager.Instance.HandleInputManagement(ref currentAction, 2, true);

        UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            //Continue
            if (currentAction == 0 && isWon && Player.CurrentStage >= 5)
            {
                Player.CurrentStage = 0;

                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            else if (currentAction == 0 && isWon)
            {
                Player.CurrentStage++;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            //Go back
            else if (currentAction == 1 && isWon)
            {
                Inventory.AllSouls += Player.CurrentSoulsCollected;
                Player.CurrentSoulsCollected = 0;
                Player.CurrentStage = 0;
                SceneManager.LoadScene(2);
            }
            else if (currentAction == 1 && !isWon)
            {
                Player.CurrentSoulsCollected = 0;
                Player.CurrentStage = 0;
                SceneManager.LoadScene(2);
            }
        }
    }

    private void UpdateActionSelection(int action)
    {
        for (int i = 0; i < options.Count; i++)
        {
            if (i == action)
            {
                options[i].color = selectedColor;
            }
            else
            {
                options[i].color = Color.black;
            }
        }
    }

    public void SetMessageText(int currentSouls, bool win)
    {
        if (win)
        {
            if (Player.CurrentStage >= 5)
            {
                messageText.text = $"Congrats! You have cleared all the stages. Wow you really made it dude im impressed. You can proceed to end screen by clicking the Continue button.";
            }
            else
            {
                messageText.text = $"Congrats! You have cleared {Player.CurrentStage} out of 5 stages in this dungeon and received {currentSouls} souls. Would you like to continue and risk your collected souls or go back to town?";
            }
            isWon = true;
        }
        else
        {
            isWon = false;
            messageText.text = $"All your Ghosts are gone... You need to return now brother. You have collected {currentSouls} souls but you lost them now. Better luck next time";
        }
    }
}
