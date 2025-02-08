using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nouveau Booster", menuName = "ScriptableObjects/Booster")]
public class Booster : ScriptableObject
{
    public List<Carte> cartes;
    public Extension extension;
    public Sprite sprite;
}
