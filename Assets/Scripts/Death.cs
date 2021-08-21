using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour
{
    public SpriteRenderer bone1;
    public SpriteRenderer bone2;
    public SpriteRenderer skull;
    public SpriteRenderer ghost;

    public void Initialize(Color color)
    {
        bone1.color = color;
        bone2.color = color;
        skull.color = color;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
