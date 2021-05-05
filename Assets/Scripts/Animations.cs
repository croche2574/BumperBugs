using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animations : MonoBehaviour
{
    Animator anim;
    float blinkTimer = 0;
    float wiggleTimer = 0;
    //Try to avoid doing some of this work if you're the fly enemy
    public bool fly = false;
    //Direction determines where the character will wiggle
    //Update this variable when a character faces a new direction
    //0 = north, 1 = east, 2 = south, 3 = west
    public int direction = 0;
    //Animate the bumps
    public bool bumped = false;
    float bumpTimer = 0.5f;
    bool moved = false;
    //Play a little noise when bumping into someone if you're the player
    AudioSource bumpSFX;
    bool hasAudio = true;

    // Start is called before the first frame update
    void Start()
    {
        anim = this.gameObject.GetComponent<Animator>();
        bumpSFX = this.gameObject.GetComponent<AudioSource>();
        if (!fly)
        {
            blinkTimer = Random.Range(2.5f, 10);
            wiggleTimer = Random.Range(5, 20);
        }
        if(bumpSFX == null)
        {
            hasAudio = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!fly)
        {
            blinkTimer -= Time.deltaTime;
            wiggleTimer -= Time.deltaTime;
            anim.SetInteger("Direction", direction);
            //Blink at random intervals
            if (blinkTimer <= 0)
            {
                //Debug.Log("Blinking!");
                blinkTimer = Random.Range(2.5f, 10);
                anim.SetInteger("Blink", 2);
            }
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Blink North") ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("Blink East") ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("Blink South") ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("Blink West"))
            {
                anim.SetInteger("Blink", 0);
            }

            //Wiggle at random intervals
            if (wiggleTimer <= 0)
            {
                //Debug.Log("Wiggling!");
                wiggleTimer = Random.Range(5, 20);
                anim.SetInteger("Wiggle", 2);
            }
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Wiggle North") ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("Wiggle East") ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("Wiggle South") ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("Wiggle West"))
            {
                anim.SetInteger("Wiggle", 0);
            }
        }
        if (bumped)
        {
            //This is annoying and bad and I hate it
            //BUT HEY, IT WORKS!
            bumpTimer -= Time.deltaTime;
            //If timer > 0, bump animation is happening
            if(bumpTimer > 0)
            {
                switch (direction)
                {
                    //North
                    case 0:
                        if (!moved)
                        {
                            transform.position += new Vector3(0, 0, 1.5f);
                            moved = true;
                            if (hasAudio)
                            {
                                bumpSFX.Play();
                            }
                            //Debug.Log("bumping!");
                        }
                        break;
                    //East
                    case 1:
                        if (!moved)
                        {
                            transform.position += new Vector3(1.5f, 0, 0);
                            moved = true;
                            if (hasAudio)
                            {
                                bumpSFX.Play();
                            }
                            //Debug.Log("bumping!");
                        }
                        break;
                    //South
                    case 2:
                        if (!moved)
                        {
                            transform.position -= new Vector3(0, 0, 1.5f);
                            moved = true;
                            if (hasAudio)
                            {
                                bumpSFX.Play();
                            }
                            //Debug.Log("bumping!");
                        }
                        break;
                    //West
                    case 3:
                        if (!moved)
                        {
                            transform.position -= new Vector3(1.5f, 0, 0);
                            moved = true;
                            if (hasAudio)
                            {
                                bumpSFX.Play();
                            }
                            //Debug.Log("bumping!");
                        }
                        break;
                    default:
                        break;
                }
            }
            //If timer < 0, bump animation is complete
            else
            {
                switch (direction)
                {
                    //North
                    case 0:
                        if (moved)
                        {
                            transform.position -= new Vector3(0, 0, 1.5f);
                            moved = false;
                            //Debug.Log("Moving back!");
                        }
                        break;
                    //East
                    case 1:
                        if (moved)
                        {
                            transform.position -= new Vector3(1.5f, 0, 0);
                            moved = false;
                            //Debug.Log("Moving back!");
                        }
                        break;
                    //South
                    case 2:
                        if (moved)
                        {
                            transform.position += new Vector3(0, 0, 1.5f);
                            moved = false;
                            //Debug.Log("Moving back!");
                        }
                        break;
                    //West
                    case 3:
                        if (moved)
                        {
                            transform.position += new Vector3(1.5f, 0, 0);
                            moved = false;
                            //Debug.Log("Moving back!");
                        }
                        break;
                    default:
                        break;
                }
                bumpTimer = 0.5f;
                bumped = false;
            }
        }
    }
}
