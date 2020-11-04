using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.HubWorld
{
    public enum State { Menu, Shop, Center, CenterMenu, Tavern, Adventure, Recruit, LvlUp, SeeParty, SwitchMonster, Busy}

    public enum ActionsInCenter { Recruit, Party, LvlUp, GoBack }

    public enum HubWorldActions { GhostCenter, Tavern, Shop, Adventure }

    public class MenuManager : MonoBehaviour
    {
        private static bool isIntroducedToShop;

        public List<Monster> playerParty = Inventory.CurrentMonsters;

        [SerializeField] private GameObject shopScreen;
        [SerializeField] private GameObject ghostCenterScreen;
        [SerializeField] private ActionsBox actionsBox;
        [SerializeField] private ActionsBox centerActionsBox;
        [SerializeField] private Animator transition;
        [SerializeField] private DialogueSystem dialogue;
        [SerializeField] private Monster genericGhost;
        [SerializeField] private PartyScreen ghostsListUI;
        [SerializeField] private List<string> tavernMessages;
        [SerializeField] private List<Monster> monstersPool;

        private InputManager inputManager = InputManager.Instance;
        private int currentAction;
        private int currentPartyUnit;
        private bool isRecruitedToday;
        private State state;
        private Monster currentSelectedMonster;
        private List<Monster> monstersForRecruit;

        private void Awake()
        {
            monstersForRecruit = new List<Monster>();
            ghostsListUI.Init();
            SetRandomMonsters();
        }

        private void Start()
        {
            transition.SetTrigger("FadeIn");
        }

        private void Update()
        {
            Debug.Log($"Current selection is {currentPartyUnit}");

            if (state == State.Menu)
            {
                actionsBox.gameObject.SetActive(true);
                centerActionsBox.gameObject.SetActive(false);
                ghostCenterScreen.SetActive(false);
                shopScreen.SetActive(false);

                inputManager.HandleInputManagement(ref currentAction, 4, false);
                HandleActionSelection();
            }
            else if (state == State.Center)
            {
                ghostCenterScreen.SetActive(true);
            }
            else if (state == State.CenterMenu)
            {
                actionsBox.gameObject.SetActive(false);
                centerActionsBox.gameObject.SetActive(true);
                ghostCenterScreen.SetActive(true);
                ghostsListUI.gameObject.SetActive(false);

                inputManager.HandleInputManagement(ref currentAction, 4, false);
                HandleActionSelectionInCenter();
            }
            else if (state == State.Recruit)
            {
                inputManager.HandleInputManagement(ref currentPartyUnit, monstersForRecruit.Count, true);
                HandleRecruitAGhost();
            }
            else if (state == State.SeeParty)
            {
                ghostsListUI.SetPartyData(Inventory.CurrentMonsters);

                inputManager.HandleInputManagement(ref currentPartyUnit, Inventory.CurrentMonsters.Count, true);
                HandleSeeParty();
                
            }
            else if (state == State.LvlUp)
            {
                ghostsListUI.gameObject.SetActive(true);
                ghostsListUI.SetPartyData(Inventory.CurrentMonsters);

                inputManager.HandleInputManagement(ref currentPartyUnit, Inventory.CurrentMonsters.Count, true);
                HandleLvlUp();
            }
            else if (state == State.SwitchMonster)
            {
                inputManager.HandleInputManagement(ref currentPartyUnit, Inventory.CurrentMonsters.Count, true);
                HandleSwitchMonster();
            }
        }

        private void SetRandomMonsters()
        {
            for (int i = 0; i < 6; i++)
            {
                Monster monster = monstersPool[UnityEngine.Random.Range(0, monstersPool.Count)];

                int rnJesus = UnityEngine.Random.Range(0, 101);

                if (rnJesus < 5)
                {
                    monster.Level = UnityEngine.Random.Range(80, 101);
                }
                else if (rnJesus < 15)
                {
                    monster.Level = UnityEngine.Random.Range(60, 80);
                }
                else if (rnJesus < 25)
                {
                    monster.Level = UnityEngine.Random.Range(40, 60);
                }
                else if (rnJesus < 50)
                {
                    monster.Level = UnityEngine.Random.Range(20, 40);
                }
                else
                {
                    monster.Level = UnityEngine.Random.Range(10, 20);
                }

                monstersForRecruit.Add(monster);
            }

            genericGhost = monstersPool[UnityEngine.Random.Range(0, monstersPool.Count)];
            genericGhost.Level = 25;
        }

        private void HandleLvlUp()
        {
            ghostsListUI.SetMessageText($"Choose which pokemon you want to lvl up. You have {Inventory.AllSouls} souls");

            ghostsListUI.UpdateUnitSelection(currentPartyUnit);

            if (Input.GetKeyDown(KeyCode.Return))
            {
                StartCoroutine(LvlUp());
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                state = State.CenterMenu;
            }
        }

        private IEnumerator LvlUp()
        {
            state = State.Busy;

            if (Inventory.AllSouls > 0)
            {
                dialogue.TypeSingleDialogue("You leveled up the Ghost");

                yield return new WaitForSeconds(2f);

                Monster monster = Inventory.CurrentMonsters[currentPartyUnit];
                monster.LevelUp();
                Inventory.AllSouls--;

                state = State.LvlUp;
            }
            else
            {
                dialogue.TypeSingleDialogue("You don't have enought souls to use level up");

                yield return new WaitForSeconds(2f);
                state = State.LvlUp;

            }
        }

        private void HandleSeeParty()
        {
            ghostsListUI.SetMessageText($"Chose a Ghost to switch his place");

            ghostsListUI.UpdateUnitSelection(currentPartyUnit);

            if (Input.GetKeyDown(KeyCode.Return))
            {
                currentSelectedMonster = Inventory.CurrentMonsters[currentPartyUnit];
                state = State.SwitchMonster;
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                currentSelectedMonster = null;
                state = State.CenterMenu;
                ghostsListUI.gameObject.SetActive(false);
            }
        }

        private void HandleSwitchMonster()
        {
            ghostsListUI.SetMessageText($"Choose a spot for that Ghost");

            ghostsListUI.UpdateUnitSelection(currentPartyUnit);

            if (Input.GetKeyDown(KeyCode.Return))
            {
                StartCoroutine(SwitchMonster());
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                currentSelectedMonster = null;
                state = State.SeeParty;
            }
        }

        private IEnumerator SwitchMonster()
        {
            state = State.Busy;

            Inventory.CurrentMonsters.Remove(currentSelectedMonster);

            Inventory.CurrentMonsters.Insert(currentPartyUnit, currentSelectedMonster);

            yield return dialogue.TypeDialogue(new List<string> { "You have switched the Ghost place" }, DialogueState.None);

            currentSelectedMonster = null;

            currentPartyUnit = 0;

            state = State.SeeParty;
        }

        private void HandleActionSelectionInCenter()
        {
            centerActionsBox.UpdateActionSelection(currentAction);

            if (currentAction == (int)ActionsInCenter.Recruit)
            {
                dialogue.TypeSingleDialogue($"See which ghosts are available for recruit today.");

                if (Input.GetKeyDown(KeyCode.Return) && !isRecruitedToday)
                {
                    state = State.Recruit;

                    ghostsListUI.gameObject.SetActive(true);
                    ghostsListUI.SetPartyData(monstersForRecruit);
                }
                else if (Input.GetKeyDown(KeyCode.Return) && isRecruitedToday)
                {
                    StartCoroutine(RecruitMonster());
                }
            }
            else if (currentAction == (int)ActionsInCenter.Party)
            {
                dialogue.TypeSingleDialogue("See what ghosts you currently have");

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    state = State.SeeParty;
                    ghostsListUI.gameObject.SetActive(true);
                    ghostsListUI.SetPartyData(Inventory.CurrentMonsters);
                    ghostsListUI.SetMessageText("Your current Ghosts party");
                }

            }
            else if (currentAction == (int)ActionsInCenter.LvlUp)
            {
                dialogue.TypeSingleDialogue("Use your souls points to level up a ghost");

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    state = State.LvlUp;
                }
            }
            else if (currentAction == (int)ActionsInCenter.GoBack)
            {
                dialogue.TypeSingleDialogue("Get back to town");

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    state = State.Menu;
                }
            }
        }

        private void HandleRecruitAGhost()
        {
            ghostsListUI.SetMessageText("Choose wisely, you can only pick one Ghost per day");

            ghostsListUI.UpdateUnitSelection(currentPartyUnit);

            if (Input.GetKeyDown(KeyCode.Return))
            {
                StartCoroutine(RecruitMonster());
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                currentPartyUnit = 0;
                state = State.CenterMenu;
            }
        }

        private IEnumerator RecruitMonster()
        {
            state = State.Busy;

            if (!isRecruitedToday)
            {
                Monster selectedMonster = monstersForRecruit[currentPartyUnit];

                monstersForRecruit.RemoveAt(currentPartyUnit);
                Inventory.CurrentMonsters.Add(selectedMonster);
                isRecruitedToday = true;

                dialogue.TypeSingleDialogue($"You have recruited {selectedMonster.MBase.MonsterName} Lvl {selectedMonster.Level}");

                yield return new WaitForSeconds(2f);

                dialogue.TypeSingleDialogue($"{selectedMonster.MBase.MonsterName} Lvl {selectedMonster.Level} is added to your party");

                yield return new WaitForSeconds(2f);

                state = State.CenterMenu;
            }
            else
            {
                yield return dialogue.TypeDialogue(new List<string> { "Sorry, you have recruited a ghost today" }, DialogueState.None);

                state = State.CenterMenu;
            }

            currentPartyUnit = 0;
        }

        private IEnumerator TavernState()
        {
            yield return dialogue.TypeDialogue(tavernMessages, DialogueState.None);

            state = State.Menu;
        }

        private void HandleActionSelection()
        {
            actionsBox.UpdateActionSelection(currentAction);
            
            if (currentAction == (int)HubWorldActions.GhostCenter)
            {
                dialogue.TypeSingleDialogue($"Go to the Ghost Center. You currently have {Inventory.AllSouls} souls");

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    ghostCenterScreen.SetActive(true);
                    StartCoroutine(GhostCenterState());
                }
            }
            else if (currentAction == (int)HubWorldActions.Tavern)
            {
                dialogue.TypeSingleDialogue("Go to the Tavern");

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    state = State.Tavern;
                    StartCoroutine(TavernState());
                }

            }
            else if (currentAction == (int)HubWorldActions.Shop)
            {
                dialogue.TypeSingleDialogue("Buy items from the shop.");

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    shopScreen.SetActive(true);
                    state = State.Shop;
                    StartCoroutine(ShopState());
                }
            }
            else if (currentAction == (int)HubWorldActions.Adventure)
            {
                dialogue.TypeSingleDialogue("Start your adventure");

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    state = State.Adventure;
                    StartCoroutine(AdventureState());
                }
            }
        }

        private IEnumerator AdventureState()
        {
            if (Inventory.CurrentMonsters.Count < 1)
            {
                List<string> messages = new List<string>();
                messages.Add("You don't have any ghosts with you... You can't just go down there like that dude!");
                messages.Add("Go to the Ghost Center, they will help you");

                yield return dialogue.TypeDialogue(messages, DialogueState.None);
                state = State.Menu;
            }
            else
            {
                transition.SetTrigger("FadeOut");

                yield return new WaitForSeconds(2f);

                SceneManager.LoadScene(3);
            }
        }

        private IEnumerator ShopState()
        {
            List<string> messages = new List<string>();
            messages.Add("Hey! What's up kid.");

            if (!isIntroducedToShop)
            {
                messages.Add("You are new here?");
                messages.Add("This is the item shop but unfortunately we are out of stock now...");
                messages.Add("But hey, you know what, we have a lot of these...");
                messages.Add("All of the heroes out there just sell these all the time and we have so much of that...");
                messages.Add("Here, take that for free. Actually i even don't know how that can help you...");
                messages.Add("If you need more you can come and get more i don't need them anyway");
                Inventory.Apples++;
                isIntroducedToShop = true;
            }
            else
            {
                messages.Add("Hey you want more apples?");
                messages.Add("Sure, take that... im happy to see that you find them useful");
                Inventory.Apples++;
            }

            yield return dialogue.TypeDialogue(messages, DialogueState.None);

            dialogue.TypeSingleDialogue("You've got an apple");

            yield return new WaitForSeconds(3f);

            state = State.Menu;
        }

        private IEnumerator GhostCenterState()
        {
            state = State.Busy;

            List<string> messages = new List<string>();
            messages.Add("Hey! Welcome to the Ghost Center.");

            if (Inventory.CurrentMonsters.Count <= 0)
            {
                messages.Add("I see you don't have any ghosts with you...");
                messages.Add("This is bad, but don't worry i will give you this one for free");
            }

            yield return dialogue.TypeDialogue(messages, DialogueState.None);

            if (Inventory.CurrentMonsters.Count <= 0)
            {
                dialogue.TypeSingleDialogue($"You received {genericGhost.MBase.MonsterName} lvl {genericGhost.Level}");
                yield return new WaitForSeconds(2f);
                dialogue.TypeSingleDialogue($"You added {genericGhost.MBase.MonsterName} to your party!");
                Inventory.CurrentMonsters.Add(genericGhost);
                yield return new WaitForSeconds(2f);
            }
            
            state = State.CenterMenu;
            

        }
    }
}