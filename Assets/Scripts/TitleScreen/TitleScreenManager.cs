using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum State { BeforeMenu, Menu, Controls}

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] private ActionsBox menuOptions;
    [SerializeField] private Animator transition;
    [SerializeField] private GameObject controlsScreen;
    [SerializeField] private GameObject subTitle;
    [SerializeField] private Text pressEnterText;

    private int currentAction;
    private bool isVerticalInUse;
    private State state;

    private void Awake()
    {
        state = State.BeforeMenu;
    }

    private void Update()
    {
        if (state == State.BeforeMenu && Input.GetKeyDown(KeyCode.Return))
        {
            pressEnterText.gameObject.SetActive(false);
            menuOptions.gameObject.SetActive(true);
            subTitle.SetActive(false);
            state = State.Menu;
        }
        else if (state == State.Menu)
        {
            HandleMenu();
        }
        else if (state == State.Controls && Input.GetKeyDown(KeyCode.Escape))
        {
            controlsScreen.SetActive(false);
            state = State.Menu;
        }
    }

    private void HandleMenu()
    {
        int axis = (int)(Input.GetAxisRaw("Vertical") * -1f);

        if (currentAction + axis <= 1 && currentAction + axis >= 0 && !isVerticalInUse)
        {
            currentAction += axis;
            isVerticalInUse = true;
        }

        menuOptions.UpdateActionSelection(currentAction);

        //Start Game
        if (currentAction == 0 && Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(StartGame());

        }
        //Controls
        else if (currentAction == 1 && Input.GetKeyDown(KeyCode.Return))
        {
            state = State.Controls;
            controlsScreen.SetActive(true);
        }
        if (Input.GetAxisRaw("Vertical") == 0)
        {
            isVerticalInUse = false;
        }
    }

    private IEnumerator StartGame()
    {
        transition.SetTrigger("FadeOut");

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene(1);
    }
}
