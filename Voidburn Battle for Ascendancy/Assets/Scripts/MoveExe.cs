using UnityEngine;
using System.Collections.Generic;
public class MoveExe : MonoBehaviour
{
    public List<FightingMoves> myMoves = new List<FightingMoves>();
    public List<KeyCode> inputbuffer = new List<KeyCode>();
    float lastInputTime;
    float inputBufferWindow;

    private KeyCode[] validList =
        {
         KeyCode.JoystickButton0,KeyCode.Joystick1Button1,KeyCode.JoystickButton2,
         KeyCode.JoystickButton3,KeyCode.JoystickButton4,KeyCode.JoystickButton5,
         KeyCode.W,KeyCode.D,KeyCode.S,KeyCode.A,KeyCode.I,KeyCode.L,KeyCode.J,KeyCode.K,
        };
    

    private void Start()
    {
        
    }


    void Update()
    {
        if(Input.anyKeyDown) //gets inputs and adds them to the buffer
        {
            foreach(KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if(Input.GetKeyDown(key))
                {
                    inputbuffer.Add(key);
                    lastInputTime = Time.time;
                }
            }
        }








    }
}
