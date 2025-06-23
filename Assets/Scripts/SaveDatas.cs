using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveDatas : MonoBehaviour
{
    public static SaveDatas Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);

        filePath = Path.Combine(Application.persistentDataPath, "datas.json");
        Debug.Log("File path: " + filePath);
    }

    [Serializable]
    public class DataWrapper
    {
        public List<ExpansionData> expansions = new List<ExpansionData>();
    }

    [Serializable]
    public class ExpansionData
    {
        public string extensionName;
        public List<CardData> cards = new List<CardData>();
    }

    [Serializable]
    public class CardData
    {
        public int cardId;
        public bool isObtained;
    }

    private string filePath;

    public void SaveCard(DisplayCard carte, bool state)
    {
        DataWrapper wrapper;
        if (File.Exists(filePath))
        {
            wrapper = LoadAll();
        }
        else
        {
            wrapper = new DataWrapper();
        }

        string extensionKey = carte.extension != null ? carte.extension.name : carte.booster.extension.name;
        // Chercher si l'extension existe déjà dans la liste des extensions
        var foundExtension = wrapper.expansions.FirstOrDefault(exp => exp.extensionName == extensionKey);

        // Si l'extension n'existe pas encore, on l'ajoute à la liste
        if (foundExtension == null)
        {
            foundExtension = new ExpansionData { extensionName = extensionKey };
            wrapper.expansions.Add(foundExtension);
        }

        // Chercher si la carte existe déjà dans cette extension
        var foundCard = foundExtension.cards.FirstOrDefault(card => card.cardId == carte.id);

        // Si la carte n'existe pas, on l'ajoute à la liste des cartes de l'extension
        if (foundCard == null)
        {
            foundExtension.cards.Add(new CardData
            {
                cardId = carte.id,
                isObtained = state
            });
        }
        else
        {
            // Si la carte existe, on met à jour son état
            foundCard.isObtained = state;
        }

        // Trier les cartes par cardId avant de les sérialiser
        foundExtension.cards = foundExtension.cards.OrderBy(card => card.cardId).ToList();

        Write(wrapper);
    }

    void Write(DataWrapper wrapper)
    {
        // Convertir en JSON
        string json = JsonUtility.ToJson(wrapper, true);

        // Sauvegarder dans un fichier
        File.WriteAllText(filePath, json);
    }

    DataWrapper LoadAll()
    {
        string json = File.ReadAllText(filePath);
        return JsonUtility.FromJson<DataWrapper>(json);
    }

    public bool Import(int cardId, string extension)
    {
        // Charger les données depuis le fichier si elles existent
        if (File.Exists(filePath))
        {
            DataWrapper wrapper = LoadAll();

            // Trouver l'extension correspondante
            var foundExtension = wrapper.expansions.FirstOrDefault(exp => exp.extensionName == extension);
            if (foundExtension != null)
            {
                // Chercher la carte dans cette extension spécifique
                var foundCard = foundExtension.cards.FirstOrDefault(card => card.cardId == cardId);
                // Si la carte est trouvée, retourner son état
                if (foundCard != null)
                {
                    return foundCard.isObtained;
                }
            }
        }
        else
        {
            Debug.Log("File doesnt exist");
        }

        // Si la carte n'est pas trouvée, retourner false (ou une valeur par défaut)
        return false;
    }
}
