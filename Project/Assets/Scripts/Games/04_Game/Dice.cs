using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    private int m_DiceNumber = -1;
    public int DiceNumber {
        get => m_DiceNumber;
        set => m_DiceNumber = value;
    }
}
