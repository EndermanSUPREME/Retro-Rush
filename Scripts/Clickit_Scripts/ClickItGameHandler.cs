using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickItGameHandler : MonoBehaviour
{
    [SerializeField] Text TargetText;
    [SerializeField] Animator TargetAnim;

    bool recordInput = false;
    char inChar, tChar;
    const string asciiNumberLower = "abcdefghijklmnopqrstuvwxyz123456789";
    RetroRushHandler MainGameHandler;
    float sTime, fTime;

    AudioSource goodClick, badClick;

    // Start is called before the first frame update
    void Start()
    {
        // Application.targetFrameRate = 65;
        MainGameHandler = GameObject.FindObjectOfType<RetroRushHandler>();
        
        goodClick = GameObject.Find("ClickItSuccess").GetComponent<AudioSource>();
        badClick = GameObject.Find("ClickItFail").GetComponent<AudioSource>();

        ReadyStage();
    }

    // Update is called once per frame
    void Update()
    {
        // get the first click within the time interval
        // input is allowed to be recorded
        if (recordInput)
        {
            // upon hit from a keyboard
            if (Input.anyKey)
            {
                if (Input.inputString.Length > 0)
                {
                    fTime = Time.time;

                    inChar = Input.inputString[0];

                    CancelInvoke("CheckClick");
                    CheckClick();
                }
            }
        }
    }

    void SpriteSwap()
    {
        DisplayNewTargetSprite();
    }

    // Can be triggered from MainHandler
    public void ReadyStage()
    {
        StartCoroutine(CountDown(1));
    }

    IEnumerator CountDown(int t)
    {
        yield return new WaitForSeconds(t);

        for (int i = 0; i < 3; ++i)
        {
            TargetText.fontSize = 50;
            TargetText.text = "Starting in " + (i+1).ToString();
            TargetAnim.Play("pop");
            yield return new WaitForSeconds(1);
        }

        Invoke("DisplayNewTargetSprite", 1);
    }

    void DisplayNewTargetSprite()
    {
        // swap the sprite and show affordance
        int tIndex = Random.Range(0, asciiNumberLower.Length - 1);
        tChar = asciiNumberLower[tIndex];

        TargetText.fontSize = 100;
        TargetText.text = "";
        TargetText.text += tChar;
        TargetText.text = TargetText.text.ToUpper();

        TargetAnim.Play("pop");

        // allow input momentarily
        HandleClickTime();
    }

    void HandleClickTime()
    {
        recordInput = true;
        sTime = Time.time;
        Invoke("CheckClick", 3);
    }

    void CheckClick()
    {
        if (inChar != tChar)
        {
            badClick.pitch = Random.Range(0.75f, 1);
            badClick.Play();
            
            MainGameHandler.LoseLife();
        } else
            {
                goodClick.pitch = Random.Range(0.75f, 1);
                goodClick.Play();

                MainGameHandler.UpdateScore(75 - (int)( (fTime - sTime) * 5));
            }

        SpriteSwap();
    }

    public void ClearText()
    {
        TargetText.text = "";
    }

    public void ReloadGame()
    {
        recordInput = false;
        ReadyStage();
    }

    public void StopHandleItems()
    {
        recordInput = false;
        CancelInvoke("CheckClick");
    }

}//EndScript