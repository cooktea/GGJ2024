using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGround : MonoBehaviour
{
    [SerializeField] bool isRestting;

    // Start is called before the first frame update
    void Start()
    {
        isRestting = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log("out ground");
        var ball = collision.gameObject.GetComponent<IBall>();
        if (!isRestting)
        {
            ball.SetOutLine(true);
            isRestting = true;
            //Debug.Log("Is out line!");
        }
    }
}
