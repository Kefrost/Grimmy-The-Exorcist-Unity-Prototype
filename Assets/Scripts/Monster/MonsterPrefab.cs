using UnityEngine;

public class MonsterPrefab : MonoBehaviour
{
    [SerializeField] private Monster monster;

    public Monster Monster => monster;

    private void Awake()
    {
        monster.Initialize(monster.Level);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player.CurrentEnemy = this;
        }
    }

    private void Update()
    {
        if (monster.HP <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
