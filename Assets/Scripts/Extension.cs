using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nouvelle extension", menuName = "ScriptableObjects/Extension")]
public class Extension : ScriptableObject
{
    public Sprite sprite;
    public List<Booster> boosters;
    public List<Carte> cartesCommunes;
    public Sprite allBoostersSprite;
}
