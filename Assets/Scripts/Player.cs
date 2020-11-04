using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{   
    public event Action OnEncounter;

    [SerializeField] private float speed = 5;
    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform battlePos;
    
    public static int CurrentSoulsCollected { get; set; }
    public static int AllSouls { get; set; }
    public static int CurrentStage { get; set; }
    public static MonsterPrefab CurrentEnemy { get; set; }

    private void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        if (CurrentStage == 0)
        {
          CurrentStage = 1;
        }
    }

    public void HandleUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(horizontal, vertical, 0).normalized;

        if (move != Vector3.zero)
        {
            anim.SetFloat("Horizontal", horizontal);
            anim.SetFloat("Vertical", vertical);
        }

        anim.SetFloat("Speed", move.sqrMagnitude);

        if (horizontal != 0)
        {
            move.y = 0;
        }

        rb.velocity = move * speed;

        if (CurrentEnemy != null)
        {
            OnEncounter?.Invoke();
            rb.velocity = Vector2.zero;
            anim.SetFloat("Speed", 0f);
        }
    }
}

public static class Inventory
{
    public static int AllSouls;

    public static int Apples;

    public static List<Monster> CurrentMonsters = new List<Monster>();

}
