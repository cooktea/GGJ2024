using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    [SerializeField] IPlayer.PlayerSide side;
    [SerializeField] GameObject gameManager;
    GameManager GM => gameManager?.GetComponent<GameManager>();

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("enter gate");
        var ball = collision.gameObject.GetComponent<IBall>();
        if (ball is not null)
        {
            GM.BallIn(side);
        }
    }
}
