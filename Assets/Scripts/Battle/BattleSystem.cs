using System;
using System.Collections;
using UnityEngine;

public enum BattleState { Start, ActionSelection, MoveSelection, ItemSelection, PerformingTurn, PartyScreen, Busy, BattleOver }

public enum BattleAction { Move, UseItem, SwitchMonster }

public class BattleSystem : MonoBehaviour
{
    public event Action<bool> OnBattleExit;

    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleUnit enemyUnit;
    [SerializeField] private BattleDialogueBox dialogueBox;
    [SerializeField] private PartyScreen partyScreen;

    private InputManager inputManager = InputManager.Instance;
    private BattleState state;
    private BattleState? prevState;
    private int currentAction;
    private int currentMoveIndex;
    private int currentItemOption;
    private int currentPartyUnit;
    private MonsterParty playerParty;
    private Monster enemyMonster;
    private bool isCheckingMoveDesc;
    private AudioSource audio;

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    public void StartBattle(MonsterParty playerParty, Monster enemyMonster)
    {
        audio.Play();
        this.playerParty = playerParty;
        this.enemyMonster = enemyMonster;
        StartCoroutine(SetupBattle());
    }

    private IEnumerator SetupBattle()
    {
        playerUnit.Setup(playerParty.GetFirstMonster());

        enemyUnit.Setup(enemyMonster);

        partyScreen.Init();

        dialogueBox.SetMoveNames(playerUnit.Monster.Moves);

        yield return dialogueBox.TypeDialogue("A ghost appeared!");

        TriggerActionSelection();
    }

    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            inputManager.HandleInputManagement(ref currentAction, 3, false);
            HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection)
        {
            inputManager.HandleInputManagement(ref currentMoveIndex, playerUnit.Monster.Moves.Count, true);
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            inputManager.HandleInputManagement(ref currentPartyUnit, playerParty.Monsters.Count, true);
            dialogueBox.EnableInfoBtnBox(false);
            HandlePartySelection();
        }
        else if (state == BattleState.ItemSelection)
        {
            inputManager.HandleInputManagement(ref currentItemOption, 2, false);
            dialogueBox.EnableDialogueText(true);
            HandleItemSelection();
        }
    }

    private void TriggerActionSelection()
    {
        state = BattleState.ActionSelection;
        dialogueBox.EnableActionBox(true);
        dialogueBox.EnableSelectBtnBox(true);
        dialogueBox.EnableInfoBtnBox(false);
        StartCoroutine(dialogueBox.TypeDialogue("Choose an action"));
    }

    private void HandleActionSelection()
    {
        dialogueBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (currentAction == (int)BattleAction.Move)
            {
                TriggerMoveSelection();
            }
            else if (currentAction == (int)BattleAction.UseItem)
            {
                TriggerItemSelection();
            }
            else if (currentAction == (int)BattleAction.SwitchMonster)
            {
                prevState = state;
                OpenPartyScreen();
            }
        }
    }

    private void TriggerMoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogueBox.EnableActionBox(false);
        dialogueBox.EnableDialogueText(false);
        dialogueBox.EnableMoveBox(true);
    }

    private void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isCheckingMoveDesc)
        {
            isCheckingMoveDesc = false;
            dialogueBox.EnableInfoBtnBox(true);
            dialogueBox.EnableSelectBtnBox(true);
            dialogueBox.EnableMoveBox(true);
            dialogueBox.EnableDescriptionText(false);
        }
        else if (!isCheckingMoveDesc)
        {
            Move currentMove = playerUnit.Monster.Moves[currentMoveIndex];

            dialogueBox.UpdateMoveSelection(currentMoveIndex, currentMove);

            if (Input.GetKeyDown(KeyCode.Return) && currentMove.PP > 0)
            {
                dialogueBox.EnableMoveBox(false);
                dialogueBox.EnableDialogueText(true);
                StartCoroutine(PerformTurns(BattleAction.Move));
            }
            else if (Input.GetKeyDown(KeyCode.Tab))
            {
                isCheckingMoveDesc = true;
                dialogueBox.EnableInfoBtnBox(false);
                dialogueBox.EnableSelectBtnBox(false);
                dialogueBox.EnableMoveBox(false);
                dialogueBox.EnableDescriptionText(true);
                dialogueBox.SetDescriptionDialogue(currentMove.Base.Description);
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                dialogueBox.EnableSelectBtnBox(true);
                dialogueBox.EnableMoveBox(false);
                dialogueBox.EnableDialogueText(true);
                dialogueBox.EnableActionBox(true);
                TriggerActionSelection();
            }
        }
    } 

    private void TriggerItemSelection()
    {
        state = BattleState.ItemSelection;

        dialogueBox.EnableItemOptions(true);
        dialogueBox.EnableActionBox(false);
        dialogueBox.TypeSingleDialogue($"You have {Inventory.Apples} apples. Do you want to use one and heal your ghost somehow...");
    }

    private void HandleItemSelection()
    {
        dialogueBox.UpdateItemOptionSelection(currentItemOption);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            // Use apple
            if (currentItemOption == 0)
            {
                if (Inventory.Apples < 1)
                {
                    state = BattleState.Busy;
                    dialogueBox.TypeDialogue("You don't have any apples");
                    state = BattleState.ItemSelection;
                }
                else
                {
                    StartCoroutine(PerformTurns(BattleAction.UseItem));
                }
            }
            // Back
            else if (currentItemOption == 1)
            {
                dialogueBox.EnableItemOptions(false);
                TriggerActionSelection();
            }
        }
    }

    private IEnumerator UseApple()
    {
        dialogueBox.EnableItemOptions(false);
        yield return dialogueBox.TypeDialogue("You gave an apple to your ghost...");

        Inventory.Apples--;

        playerUnit.Monster.CureStatus();
        playerUnit.Monster.CureVolatileStatus();
        playerUnit.Monster.Heal(playerUnit.Monster.MaxHp / 4);
        yield return playerUnit.Hud.UpdateHp();
    }

    private void HandlePartySelection()
    {
        partyScreen.UpdateUnitSelection(currentPartyUnit);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Monster selectedMonster = playerParty.Monsters[currentPartyUnit];

            if (playerParty.Monsters[currentPartyUnit].HP <= 0)
            {
                partyScreen.SetMessageText("You cannot send out a fainted monster.");
                return;
            }

            if (selectedMonster == playerUnit.Monster)
            {
                partyScreen.SetMessageText("You cannot switch ith the same monster.");
                return;
            }

            partyScreen.gameObject.SetActive(false);

            if (prevState == BattleState.ActionSelection)
            {
                prevState = null;
                StartCoroutine(PerformTurns(BattleAction.SwitchMonster));
            }
            else
            {
                state = BattleState.Busy;
                StartCoroutine(SwitchMonster(selectedMonster));
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && playerUnit.Monster.HP > 0)
        {
            partyScreen.gameObject.SetActive(false);
            TriggerActionSelection();
        }
    }

    private IEnumerator SwitchMonster(Monster newMonster)
    {
        if (playerUnit.Monster.HP > 0)
        {
            currentMoveIndex = 0;
            yield return dialogueBox.TypeDialogue($"Get back {playerUnit.Monster.MBase.MonsterName}!");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }

        playerUnit.Setup(newMonster);

        dialogueBox.SetMoveNames(newMonster.Moves);

        yield return dialogueBox.TypeDialogue($"Go {newMonster.MBase.MonsterName}!");

        state = BattleState.PerformingTurn;
    }

    public IEnumerator PerformTurns(BattleAction playerAction)
    {
        state = BattleState.PerformingTurn;

        if (playerAction == BattleAction.Move)
        {
            playerUnit.Monster.CurrentMove = playerUnit.Monster.Moves[currentMoveIndex];
            enemyUnit.Monster.CurrentMove = enemyUnit.Monster.GetRandomMove();

            int playerMovePriority = playerUnit.Monster.CurrentMove.Base.Priority;
            int enemyMovePriority = enemyUnit.Monster.CurrentMove.Base.Priority;

            // Check who goes first
            bool playerGoesFirst = true;

            if (enemyMovePriority > playerMovePriority)
            {
                playerGoesFirst = false;
            }
            else if (enemyMovePriority == playerMovePriority)
            {
                playerGoesFirst = playerUnit.Monster.Speed >= enemyUnit.Monster.Speed;
            }

            BattleUnit firstUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
            BattleUnit secondUnit = (playerGoesFirst) ? enemyUnit : playerUnit;

            Monster secondMonster = secondUnit.Monster;

            //First Turn
            yield return PerformMove(firstUnit, secondUnit, firstUnit.Monster.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (state == BattleState.BattleOver)
            {
                yield break;
            }

            if (secondMonster.HP > 0)
            {
                //Second Turn
                yield return PerformMove(secondUnit, firstUnit, secondUnit.Monster.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (state == BattleState.BattleOver)
                {
                    yield break;
                }
            }
        }
        else if (playerAction == BattleAction.UseItem)
        {
            yield return UseApple();

            //enemy turn is called after item use
            Move enemyMove = enemyUnit.Monster.GetRandomMove();
            yield return PerformMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurn(enemyUnit);
            if (state == BattleState.BattleOver)
            {
                yield break;
            }
        }
        //The action is switch monster
        else if(playerAction == BattleAction.SwitchMonster)
        {
            var selectedMonster = playerParty.Monsters[currentPartyUnit];
            state = BattleState.Busy;
            yield return SwitchMonster(selectedMonster);
            
            //enemy attacks after the switching of the monster
            Move enemyMove = enemyUnit.Monster.GetRandomMove();
            yield return PerformMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurn(enemyUnit);
            if (state == BattleState.BattleOver)
            {
                yield break;
            }
        }

        if (state != BattleState.BattleOver)
        {
            TriggerActionSelection();
        }
    }

    private IEnumerator PerformMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        bool canPerformMove = sourceUnit.Monster.OnBeforeTurn();
        if (!canPerformMove)
        {
            yield return ShowStatusChanges(sourceUnit.Monster);
            yield return sourceUnit.Hud.UpdateHp();
            yield break;
        }
        yield return ShowStatusChanges(sourceUnit.Monster);

        move.PP--;

        yield return dialogueBox.TypeDialogue($"{sourceUnit.Monster.MBase.MonsterName} used {move.Base.MoveName}");

        if (CheckMoveHits(move, sourceUnit.Monster, targetUnit.Monster))
        {
            sourceUnit.PlayAttackAnimation();
            yield return new WaitForSeconds(1f);

            targetUnit.PlayHitAnimation();

            if (move.Base.Category == MoveCategory.Status)
            {
                yield return PerformStatusMove(sourceUnit.Monster, targetUnit.Monster, move.Base.Effects, move.Base.Target);
            }
            else
            {
                DamageDetails dmgDetails = targetUnit.Monster.TakeDamage(move, sourceUnit.Monster);
                yield return targetUnit.Hud.UpdateHp();
                yield return TypeDamageDetails(dmgDetails);
            }

            if (move.Base.SecondaryMoveEffects != null && move.Base.SecondaryMoveEffects.Count > 0 && targetUnit.Monster.HP > 0)
            {
                foreach (var effect in move.Base.SecondaryMoveEffects)
                {
                    int rnd = UnityEngine.Random.Range(1, 101);
                    if (rnd <= effect.Chance)
                    {
                        yield return PerformStatusMove(sourceUnit.Monster, targetUnit.Monster, effect, effect.Target);
                    }
                }
            }
        }
        else
        {
            yield return dialogueBox.TypeDialogue($"{sourceUnit.Monster.MBase.MonsterName}'s attack missed");
        }

        yield return CheckForFaint(targetUnit);
    }

    private IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if (state == BattleState.BattleOver)
        {
            yield break;
        }

        yield return new WaitUntil(() => state == BattleState.PerformingTurn);

        //When there is status like poison or burn, it will hurt the monster after the turn
        sourceUnit.Monster.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Monster);
        yield return sourceUnit.Hud.UpdateHp();
        yield return CheckForFaint(sourceUnit);
    }

    private IEnumerator CheckForFaint(BattleUnit battleUnit)
    {
        int prize = UnityEngine.Random.Range(1 * Player.CurrentStage, 3 * Player.CurrentStage + 1);

        if (battleUnit.Monster.HP <= 0)
        {
            yield return dialogueBox.TypeDialogue($"{battleUnit.Monster.MBase.MonsterName} fainted");

            if (battleUnit != playerUnit)
            {
                yield return dialogueBox.TypeDialogue($"You've got {prize} souls");
                Player.CurrentSoulsCollected += prize;
            }

            battleUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(2.5f);

            CheckForBattleExit(battleUnit);
        }
    }

    private bool CheckMoveHits(Move move, Monster source, Monster target)
    {
        if (move.Base.AlwaysHits)
        {
            return true;
        }

        float moveAccuracy = move.Base.Accuracy;

        int accuracy = source.StatBoosts[Stat.Accuracy];
        int evasion = target.StatBoosts[Stat.Evasion];

        float[] boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (accuracy > 0)
        {
            moveAccuracy *= boostValues[accuracy];
        }
        else
        {
            moveAccuracy /= boostValues[-accuracy];
        }

        if (evasion > 0)
        {
            moveAccuracy /= boostValues[evasion];
        }
        else
        {
            moveAccuracy *= boostValues[-evasion];
        }

        return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
    }

    private IEnumerator PerformStatusMove(Monster source, Monster target, MoveEffects effects, MoveTarget moveTarget)
    {
        if (effects.Boosts != null)
        {
            if (moveTarget == MoveTarget.Self)
            {
                source.ApplyBoosts(effects.Boosts);
            }
            else
            {
                target.ApplyBoosts(effects.Boosts);
            }
        }

        if (effects.Status != ConditionId.None)
        {
            target.SetStatus(effects.Status);
        }

        if (effects.VolatileStatus != ConditionId.None)
        {
            target.SetVolatileStatus(effects.VolatileStatus);
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    private IEnumerator ShowStatusChanges(Monster monster)
    {
        while (monster.StatusChanges.Count > 0)
        {
            string message = monster.StatusChanges.Dequeue();

            yield return dialogueBox.TypeDialogue(message);
        }
    }

    private void CheckForBattleExit(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            Inventory.CurrentMonsters.Remove(faintedUnit.Monster);

            Monster nextMonster = playerParty.GetFirstMonster();

            if (nextMonster != null)
            {
                OpenPartyScreen();
            }
            else
            {
                BattleOver(false);
            }
        }
        else
        {
            BattleOver(true);
        }
    }

    private void BattleOver(bool won)
    {
        state = BattleState.BattleOver;

        playerParty.Monsters.ForEach(m => m.OnBattleOver());

        OnBattleExit?.Invoke(won);
    }

    private IEnumerator TypeDamageDetails(DamageDetails dmgDetails)
    {
        if (dmgDetails.Critical > 1)
        {
            yield return dialogueBox.TypeDialogue("A critical hit!");
        }

        if (dmgDetails.Effectiveness > 1)
        {
            yield return dialogueBox.TypeDialogue("It's super effective!");
        }
        else if (dmgDetails.Effectiveness == 0)
        {
            yield return dialogueBox.TypeDialogue("It has no effect!");
        }
        else if (dmgDetails.Effectiveness < 1)
        {
            yield return dialogueBox.TypeDialogue("It's not very effective!");
        }
    }

    private void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Monsters);
        partyScreen.gameObject.SetActive(true);
    }
}
