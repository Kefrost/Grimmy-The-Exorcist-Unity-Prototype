using System.Linq;
using UnityEngine;

public enum GameState { Roaming, Battle, Win, Lose}

public class GameManager : MonoBehaviour
{
    [SerializeField] private Animator transition;
    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private Camera worldCam;
    [SerializeField] private GameObject enemiesContainer;
    [SerializeField] private Player playerController;
    [SerializeField] private UIMenuBox youWinScreen;

    private GameState state;
    private MonsterParty monsterParty;

    private void Awake()
    {
        ConditionsDb.Init();
    }

    private void Start()
    {
        transition.SetTrigger("FadeIn");

        playerController.OnEncounter += StartBattle;
        battleSystem.OnBattleExit += ExitBattle;
        monsterParty = playerController.GetComponent<MonsterParty>();
        monsterParty.InitializeMonsters();
    }

    private void ExitBattle(bool won)
    {
        Player.CurrentEnemy = null;
        if (!won)
        {
            state = GameState.Lose;
        }
        else
        {
            state = GameState.Roaming;
            battleSystem.gameObject.SetActive(false);
            worldCam.gameObject.SetActive(true);
        }
    }

    private void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCam.gameObject.SetActive(false);
        var enemy = Player.CurrentEnemy;
        battleSystem.StartBattle(monsterParty, enemy.Monster);
    }

    private void Update()
    {
        if (state == GameState.Roaming)
        {
            playerController.HandleUpdate();
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (state == GameState.Win)
        {
            youWinScreen.HandleUpdate();
        }
        else if (state == GameState.Lose)
        {
            Player.CurrentStage = 0;
            youWinScreen.SetMessageText(Player.CurrentSoulsCollected, false);
            youWinScreen.gameObject.SetActive(true);
            youWinScreen.HandleUpdate();
        }

        if ((!enemiesContainer.gameObject.GetComponentsInChildren<MonsterPrefab>().Any() && state == GameState.Roaming))
        {
            state = GameState.Win;
            youWinScreen.SetMessageText(Player.CurrentSoulsCollected, true);
            youWinScreen.gameObject.SetActive(true);
        }
    }
}
