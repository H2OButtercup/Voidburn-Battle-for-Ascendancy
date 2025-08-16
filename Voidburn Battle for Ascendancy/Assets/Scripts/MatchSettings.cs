using JetBrains.Annotations;
using UnityEngine;
[CreateAssetMenu]
public class MatchSettings : ScriptableObject
{
    int playerOneWins = 0;
    int playerTwoWins = 0;

    float roundTimer;
    int bestOf;
    int winsneeded;
}
