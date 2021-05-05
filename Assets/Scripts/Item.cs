using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
public class Item : NetworkComponent
{
    public int Value;
    public GameSquare CurrentSquare;
    public AudioSource Bite;
    public override void HandleMessage(string flag, string value)
    {
        
    }
    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(1f);
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
