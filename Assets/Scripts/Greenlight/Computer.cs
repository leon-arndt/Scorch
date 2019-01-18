using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : MonoBehaviour {
    GameObject computer;

    /*public void ShowComputer()
    {
        Debug.Log("The computer is being shown");
        playerController.AllowMouseLook(false);
        playerController.SetMemoboardOpen(true);
        computer.SetActive(true);
        loginScreen.SetActive(true);
        emailsScreen.SetActive(false);
        SetWrongPasswordTextVisibility(false);
    }

    public void HideComputer()
    {
        computer.SetActive(false);
        playerController.AllowMouseLook(true);
        playerController.SetMemoboardOpen(false);
    }

    public void ShowEmailsIfPasswordIsCorrect()
    {
        if (loginInputField.text == "1234")
        {
            emailsScreen.SetActive(true);
        }
        else
        {
            SetWrongPasswordTextVisibility(true);
        }
    }


    public void UpdateEmailContents(string str)
    {
        emailContents.text = str;
        //reset the email attachment
        emailAttachment.sprite = null;
    }

    public void UpdateEmailAttachment(Sprite spr)
    {
        if (spr != null)
        {
            emailAttachment.sprite = spr;
        }
    }*/
}
