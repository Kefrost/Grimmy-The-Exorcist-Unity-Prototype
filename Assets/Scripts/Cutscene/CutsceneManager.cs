using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] private DialogueSystem dialogue;
    [SerializeField] private Animator transition;

    private void Start()
    {
        transition.SetTrigger("FadeIn");

        StartCoroutine(StartCutscene());
    }

    private IEnumerator StartCutscene()
    {
        List<string> messages = new List<string>();
        messages.Add("Hey, wake up kid");
        messages.Add("Are you ok, oh god i don't wanna be responsible for you...");
        messages.Add("Just wake up bro...");
        messages.Add("Come on bro...");
        messages.Add("Hey kid you are starting to get on my nerves...");
        messages.Add("CAN YOU WAKE UP, LIKE WTF THIS IS NOT FRICKING BREATH OF THE WILD AND IM NOT THE STUPID PRINCESS WHO IS WHISPERING IN YOUR EARS TO WAKE UP AND GO FOR A QUEST...................");
        messages.Add("Dude seriously i will punch you maybe that will help...");
        messages.Add("*punch sound* (i don't have the budget to buy quality sound effects)");
        messages.Add("Oh you finally wake up...");
        messages.Add("Hmmm i wonder who you might be...");
        messages.Add("Hmmmmmmmm");
        messages.Add("Oh wait i remember now!");
        messages.Add("You must be the hero that will save us from the stupid ghosts in the town");
        messages.Add("But i didn't excpect that the hero will be a stupid kid...");
        messages.Add("Ok look, i will try to explain everything fast because you are probably already pissed off by that stupid dialogue...");
        messages.Add("Sooo there are ghosts here ok? And we don't like ghosts... So you need to go down in the dungeons where they are hiding at daytime and kill them... ALL OF THEM OK??!?!");
        messages.Add("Maybe you are wondering how you will defeat them and the answer is simple...");
        messages.Add("WITH GHOSTS!!!");
        messages.Add("Wait you are telling me this dosn't make sense??");
        messages.Add("I don't care kid this is some random rushed game made for a week, actually what do you expect, to play something meaningful and exciting like Wotcher 3??");
        messages.Add("You know actually now as i mentioned that game, my creator hasn't played that game yet... Damn he is strange dude...");
        messages.Add("Ok anyway so yeah as i said the game loop is simple you know...");
        messages.Add("Go in the town get some friendly ghosts in your team, probably purchase some items if my creator actually implement something like this... and then go and fight the evil ghost it's simple right?");
        messages.Add("And as i mentioned implementing stuff... actually im really sorry if this is the hundredth time you see this pointless conversation, but my creator actually didn't put any save system in this game sooo...");
        messages.Add("DON'T CLOSE YOUR BROWSER OK?");
        messages.Add("Because you will need to see this whole stupid dialogue again and im sure you don't want that...");
        messages.Add("Ok now go for your new adventure kid.");

        yield return dialogue.TypeDialogue(messages, DialogueState.Cutscene);

        transition.SetTrigger("FadeOut");

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(2);
    }
}
