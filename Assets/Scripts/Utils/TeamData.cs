using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Team Data", menuName = "Ludo/Team Data")]
public class TeamData : ScriptableObject
{
    public string teamName;
    public string teamHexCode;
}
