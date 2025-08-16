using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class FightingMoves 
{
    public string Name;
    public KeyCode[] inputSequence; // everything that isnt jabs and kicks
    public int damage;
    public float coolDown; // so they cant spam
    public AnimationClip animation;
    public bool isSpecial;


    public bool InputCheck(List<KeyCode> inputs)
    {

        if(inputs.Count < inputSequence.Length)return false;

        for (int i = 0; i < inputs.Count; i++)
        {
            if (inputs[inputs.Count - inputSequence.Length + i] != inputSequence[i])
                return false;

        }
        return true;
    }




}
