using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] private int level;
    [SerializeField] private bool isPlayerUnit;
    [SerializeField] private BattleHud hud;
    [SerializeField] private MonsterBase mBase;

    private Image image;
    private Vector3 originalPos;
    private Color originalColor;
    private AudioSource audio;

    public BattleHud Hud => hud;
    public Monster Monster { get; private set; }
    public bool IsPlayerUnit => isPlayerUnit;

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
        audio.pitch = 2f;
        image = GetComponent<Image>();
        originalPos = image.transform.position;
        originalColor = image.color;
    }

    public void Setup(Monster monster)
    {
        Monster = monster;

        if (isPlayerUnit)
        {
            image.sprite = Monster.MBase.BackSprite;
        }
        else
        {
            image.sprite = Monster.MBase.FrontSprite;
        }

        image.color = originalColor;

        PlayEnterAnimation();

        hud.SetData(monster);
    }

    public void PlayEnterAnimation()
    {
        if (isPlayerUnit)
        {
            image.transform.position = new Vector3(originalPos.x - 50f, originalPos.y);
        }
        else
        {
            image.transform.position = new Vector3(originalPos.x + 50f, originalPos.y);
        }

        image.transform.DOMoveX(originalPos.x, 1f);
    }

    public void PlayAttackAnimation()
    {
        Sequence sequence = DOTween.Sequence();

        if (isPlayerUnit)
        {
            sequence.Append(image.transform.DOMoveX(originalPos.x + 1f, 0.2f));
        }
        else
        {
            sequence.Append(image.transform.DOMoveX(originalPos.x - 1f, 0.2f));
        }

        sequence.Append(image.transform.DOMoveX(originalPos.x, 0.2f));
    }

    public void PlayHitAnimation()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(image.DOColor(Color.grey, 0.1f));
        sequence.Append(image.DOColor(originalColor, 0.1f));
    }

    public void PlayFaintAnimation()
    {
        audio.Play();
        image.DOFade(0f, 3f);
    }
}
