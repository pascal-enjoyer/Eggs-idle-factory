using UnityEngine;

[CreateAssetMenu(fileName = "EggData", menuName = "Game/EggData", order = 1)]
public class EggData : ScriptableObject
{
    public Sprite EggSprite;
    public string EggName;
    public int Income = 1;
}