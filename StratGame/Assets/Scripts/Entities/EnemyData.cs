using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Entity>().time = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ProcessTurn()
    {
        Debug.Log(name + " is ending its turn");
    }

    
}
