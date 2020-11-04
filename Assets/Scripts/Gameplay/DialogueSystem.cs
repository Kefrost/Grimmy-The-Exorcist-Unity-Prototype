using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum DialogueState { None, Cutscene}

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] private int lettersPerSecond;
    [SerializeField] private Text dialogueText;

    private int clickedButtons;
    private bool isDisplaySkipMessage;
    private AudioSource audio;

    private List<string> skipMessages = new List<string>() { "Can you stop punching at your keyboard im trying to explain what the hell this stupid game is about...", "Stop clicking the buttons while i'm talking bro... You can't skip the dialogue...", "Oh i bet you really wanna skip this dialogue, don't you?", "It's soo funny to see you mashing at your keyboard trying to skip this unskippable dialogue AND MESSING WITH THE CONVERSATION...", "OK, ok fine i get it... probably this is the thousandth time you see this and just wanna skip... I hope that you have read the original dialogue at least once because it's really important. Here we go SKIP" };

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
        audio.loop = false;
    }

    public IEnumerator TypeDialogue(List<string> messages, DialogueState state)
    {
        foreach (var message in messages)
        {
            dialogueText.text = "";

            foreach (var letter in message)
            {
                audio.Play();
                dialogueText.text += letter;
                yield return new WaitForSeconds(1f / lettersPerSecond);
            }

            yield return new WaitForSeconds(0.2f);
            dialogueText.text += " >>>";

            if (isDisplaySkipMessage)
            {
                isDisplaySkipMessage = false;
                skipMessages.RemoveAt(0);
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape));
                if (!skipMessages.Any())
                {
                    SceneManager.LoadScene(2);
                }
                break;
            }

            if (state != DialogueState.Cutscene)
            {
                clickedButtons = 0;
            }

            if (clickedButtons < 10)
            {
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape));
            }

            if (clickedButtons >= 10 && skipMessages.Any() && state == DialogueState.Cutscene)
            {
                isDisplaySkipMessage = true;
                clickedButtons = 0;
                yield return TypeDialogue(skipMessages, state);
            }

            clickedButtons = 0;
        }
    }

    public void TypeSingleDialogue(string message)
    {
        dialogueText.text = message;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Return))
        {
            clickedButtons++;
        }

        Debug.Log($"you have clicked buttons {clickedButtons} times");
    }
}
