﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    
    public float factor = 0.01f;
    public float jumpAmount = 0.9f;

    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;

    public GameObject clones;
    public CloneMove[] cloneMoves;

    private bool canJump;

    private int keyCnt;
    [SerializeField] int teleKeyCnt;
    [SerializeField] UnityEngine.UI.Text keyCntTxt;
    [SerializeField] UnityEngine.UI.Text winTxt;
    [SerializeField] UnityEngine.UI.Text lostTxt;

    [SerializeField] GameObject destinationDoor;
    [SerializeField] AudioSource collectSthSFX;

    private Vector3 moveVector;
    void Start()
    {
        keyCnt = 0;
        cloneMoves = clones.GetComponentsInChildren<CloneMove>();

        canJump = true;
        moveVector = new Vector3(1 * factor, 0, 0);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += moveVector;

            MoveClones(moveVector, true);

            spriteRenderer.flipX = false;

        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= moveVector;

            MoveClones(moveVector, false);

            spriteRenderer.flipX = true;
        }

        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            rb.AddForce(transform.up * jumpAmount, ForceMode2D.Impulse);
            JumpClones(jumpAmount);
        }


        // This was added to answer a question.
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Destroy(this.gameObject);
        }


        // This is too dirty. We must decalare/calculate the bounds in another way. 
        /*if (transform.position.x < -0.55f) 
        {
            transform.position = new Vector3(0.51f, transform.position.y, transform.position.z);
        }
        else if (transform.position.x > 0.53f)
        {
            transform.position = new Vector3(-0.53f, transform.position.y, transform.position.z);
        }*/
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(TagNames.DeathZone.ToString()))
        {
            lostTxt.text = "You Lost :(";
            Debug.Log("DEATH ZONE");
        }
        
        if (collision.gameObject.CompareTag(TagNames.CollectableItem.ToString()))
        {
            collision.gameObject.SetActive(false);
            Debug.Log("POTION!");
            collectSthSFX.Play();
        }

        if (collision.gameObject.CompareTag(TagNames.Key.ToString()) )
        {
            if (Input.GetKey(KeyCode.E))
            {
                collision.gameObject.SetActive(false);
                keyCnt++;
                Debug.Log(keyCnt.ToString() + " Keys Collected!");
                keyCntTxt.text = ("Keys: " + keyCnt.ToString());
                collectSthSFX.Play();
            }
        }

        if(collision.gameObject.CompareTag(TagNames.TeleportKey.ToString()) && Input.GetKey(KeyCode.E))
        {
            collision.gameObject.SetActive(false);
            teleKeyCnt++;
            collectSthSFX.Play();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(TagNames.StickyPlatform.ToString()))
        {
            Debug.LogWarning("sticky");
            canJump = false;
        }

        if (collision.gameObject.CompareTag(TagNames.ExitDoor.ToString()) &&
            Input.GetKey(KeyCode.E) && keyCnt >= 2)
        {
            winTxt.text = "You Won!";
            Debug.Log("Exit Door");
        }

        if (collision.gameObject.CompareTag(TagNames.TeleSource.ToString()) && Input.GetKey(KeyCode.E)
            && teleKeyCnt >= 1)
        {
            Debug.Log("Here is source door!");
            teleKeyCnt--;
            Vector3 detinationDoorPos = destinationDoor.transform.position;
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            gameObject.transform.position = detinationDoorPos;
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
        }
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(TagNames.StickyPlatform.ToString()))
        {
            Debug.LogWarning("sticky no more bruh");
            canJump = true;
        }
    }


    public void MoveClones(Vector3 vec, bool isDirRight)
    {
        foreach (var c in cloneMoves)
            c.Move(vec, isDirRight);
    }


    public void JumpClones(float amount)
    {
        foreach (var c in cloneMoves)
            c.Jump(amount);
    }
}
