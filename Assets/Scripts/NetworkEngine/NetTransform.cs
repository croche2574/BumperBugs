using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NETWORK_ENGINE;

public class NetTransform : NetworkComponent
{
    //Position, velocity, rotation, angular velocity
    public Vector3 LastPosition;
    public Vector3 LastRotation;
    public Vector3 NewPosition;
    public Vector3 PrevPosition = Vector3.zero;
    public float speed = 1.0f;
    public float Threshold = .1f;
    public float EThreshold = 6.5f;

    public static Vector3 StringToVector3(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith ("(") && sVector.EndsWith (")")) {
            sVector = sVector.Substring(1, sVector.Length-2);
        }
 
         // split the items
         string[] sArray = sVector.Split(',');
 
         // store as a Vector3
         Vector3 result = new Vector3(
             float.Parse(sArray[0]),
             float.Parse(sArray[1]),
             float.Parse(sArray[2]));
 
         return result;
    }

    public override void HandleMessage(string flag, string value)
    {
        if(flag == "POS" && IsClient)
        {
            //Parse out our position
            //Update LastPosition
            LastPosition = StringToVector3(value);
            //Find threshold.
            //Asssuming we are below the emergency 
            float d = (this.transform.position - LastPosition).magnitude;
            //Debug.Log(d);
            if(d > EThreshold)
            {
                Debug.Log("Emergency");
                gameObject.transform.position = LastPosition;
            }
            else if (d > Threshold)
            {
                //NewPosition = StringToVector3(value);
            }
            else
            {
                //Do nothing
            }
            
        }
        if(flag == "ROT" && IsClient)
        {
            //Parse out rotaiton and set it.
            Vector3 tempRot = Vector3.zero;
            tempRot = StringToVector3(value);
            gameObject.transform.rotation = Quaternion.Euler(tempRot);
            
        }
    }

    public override IEnumerator SlowUpdate()
    {
        if (IsClient && gameObject.GetComponent<NavMeshAgent>())
        {
            gameObject.GetComponent<NavMeshAgent>().enabled = false;
        }
        while(true)
        {
            if(IsServer)
            {
                //Is the difference in position > threshold - If so send
                if ((gameObject.transform.position - LastPosition).magnitude > Threshold)
                {
                    SendUpdate("POS", gameObject.transform.position.ToString());
                }
                //Is the difference in rotation > threshold - if so send.
                if ((gameObject.transform.rotation.eulerAngles - LastRotation).magnitude > Threshold)
                {
                    SendUpdate("ROT", gameObject.transform.rotation.eulerAngles.ToString());
                }
                if (IsDirty)
                {
                    //Send rigid body position
                    SendUpdate("POS", gameObject.transform.position.ToString());
                    //Send rigid body rotation
                    SendUpdate("ROT", gameObject.transform.rotation.eulerAngles.ToString());
                    IsDirty = false;
                }
                LastPosition = gameObject.transform.position;
                LastRotation = gameObject.transform.rotation.eulerAngles;
            }
            yield return new WaitForSeconds(.1f);
            
        }
        
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void FixedUpdate()
    {
        if (IsClient)
        {
            //float step =  speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, LastPosition, Time.deltaTime*20);
        }
    }
}
