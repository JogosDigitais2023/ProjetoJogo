using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public bool estaNoChao;
    private BoxCollider2D coll;
    private Rigidbody2D rigidBody;
    [SerializeField] private Transform player;

    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private LayerMask jumpableGround;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        run();
        EstaNoChao();
    }

    void run()
    {
        Vector2 posicaoPlayer = player.position;
        Vector2 posicaoInimigo = rigidBody.position;
        Vector2 direcao = posicaoPlayer - posicaoInimigo;
        direcao = direcao.normalized;

        if (direcao.x >= 0)
        {
            rigidBody.velocity = (moveSpeed * direcao);
        }
    }

    void EstaNoChao()
    {
        bool result = IsGrounded();
        estaNoChao = result;
        //Debug.Log("Esta no chao: " + result);
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, 0.1f, jumpableGround);
    }
}
