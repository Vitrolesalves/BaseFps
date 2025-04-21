using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    [SerializeField] private GameObject escMenu;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            EscMenuOn();
        }
        
    }

    void HandlerMouseState()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                UnlockCursor();
            }

        }
        if (Cursor.lockState == CursorLockMode.None && Input.GetMouseButtonDown(0))
        {

            LockCursor();
        }
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("Cursor Locked and Hidden");
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("Cursor Unlocked and Visible");
    }

    void EscMenuOn()
    {
        escMenu.SetActive(true);
        UnlockCursor();
    }

    void EscMenuOff()
    {
        escMenu.SetActive(false);
        LockCursor();
    }

}
