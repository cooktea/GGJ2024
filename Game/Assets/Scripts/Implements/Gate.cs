using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    [SerializeField] bool Enemy;
    [SerializeField] bool isScoring;

    Collider2D Collider;
    // Start is called before the first frame update
    void Start()
    {
        Collider = GetComponent<Collider2D>();
        isScoring = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("enter gate");
        var ball = collision.gameObject.GetComponent<IBall>();
        if (!isScoring && ball is not null)
        {
            ball.SetIsScore(true);
            isScoring = true;
        }
    }
}
