using UnityEngine;

[CreateAssetMenu(fileName = "Nouvelle Carte", menuName = "ScriptableObjects/Carte")]
public class Carte : ScriptableObject
{
    public int id;
    public Sprite sprite;
    public RarityManager.Rarity rarity;
    public Booster booster;
    public Extension extension;
}
