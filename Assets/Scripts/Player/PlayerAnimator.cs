using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    Animator am;
    PlayerMovement pm;
    SpriteRenderer sr;

    string characterName;

    // Start is called before the first frame update
    void Start()
    {
        am = GetComponent<Animator>();
        pm = GetComponent<PlayerMovement>();
        sr = GetComponent<SpriteRenderer>();
        characterName = CharacterSelector.GetData().Name;

        if (CharacterSelector.instance)
            CharacterSelector.instance.DestroySingleton();
    }

    // Update is called once per frame
    void Update()
    {
        if (pm.moveDir.x != 0 || pm.moveDir.y != 0)
        {
            am.enabled = true;
            //Debug.Log(characterName);
            switch (characterName)
            {
                case "Świeżak":
                    am.SetBool("Move1", true);
                    break;
                case "Zaprawiony":
                    am.SetBool("Move2", true);
                    break;
                case "Wyjadacz":
                    am.SetBool("Move3", true);
                    break;
            }

            SpriteDirectionChecker();
        }
        else
        {
            switch (characterName)
            {
                case "Świeżak":
                    am.SetBool("Move1", false);
                    break;
                case "Zaprawiony":
                    am.SetBool("Move2", false);
                    break;
                case "Wyjadacz":
                    am.SetBool("Move3", false);
                    break;
            }
            am.enabled = false;

        }
    }

    void SpriteDirectionChecker()
    {
        if (pm.lastHorizontalVector < 0)
            sr.flipX = true;
        else
            sr.flipX = false;
    }
}
