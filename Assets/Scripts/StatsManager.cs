using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class StatsManager : MonoBehaviour
{
    public Manager manager;
    public List<DisplayCard> missingCards;

    public List<Manager.ExtensionComposition> extensionMissingCompositions = new List<Manager.ExtensionComposition>();
    Dictionary<string, Manager.ExtensionComposition> extensionMissingCompositionsDict = new Dictionary<string, Manager.ExtensionComposition>();

    public Dictionary<string, int> numberOfCards = new Dictionary<string, int>();
    public Dictionary<string, int> numberOfSecretsCards = new Dictionary<string, int>();

    public Dictionary<string, int> numberOfObtainedCards = new Dictionary<string, int>();


    void Awake()
    {
        foreach (Extension extension in manager.extensions)
        {
            extensionMissingCompositions.Add(new Manager.ExtensionComposition(extension.name));
            numberOfCards.Add(extension.name, 0);
            numberOfSecretsCards.Add(extension.name, 0);
            numberOfObtainedCards.Add(extension.name, 0);
            if (extension.cartesCommunes.Count != 0)
            {
                extensionMissingCompositions[extensionMissingCompositions.Count - 1].boostersCompositions.Add(new Manager.BoostersComposition("Cartes communes"));
                foreach (RarityManager.Rarity rarity in Enum.GetValues(typeof(RarityManager.Rarity)))
                {
                    extensionMissingCompositions[extensionMissingCompositions.Count - 1].boostersCompositions[extensionMissingCompositions[extensionMissingCompositions.Count - 1].boostersCompositions.Count - 1].rarityPerBoosters.Add(new Manager.RarityPerBoosters(rarity, 0));
                }
            }

            foreach (Booster booster in extension.boosters)
            {
                extensionMissingCompositions[extensionMissingCompositions.Count - 1].boostersCompositions.Add(new Manager.BoostersComposition(booster.name));
                foreach (RarityManager.Rarity rarity in Enum.GetValues(typeof(RarityManager.Rarity)))
                {
                    extensionMissingCompositions[extensionMissingCompositions.Count - 1].boostersCompositions[extensionMissingCompositions[extensionMissingCompositions.Count - 1].boostersCompositions.Count - 1].rarityPerBoosters.Add(new Manager.RarityPerBoosters(rarity, 0));
                }
            }
        }
        extensionMissingCompositionsDict = extensionMissingCompositions.ToDictionary(ext => ext.name);
    }

    public void AddCardToMissingList(DisplayCard card)
    {
        if (!card.isObtained)
        {
            missingCards.Add(card);

            if (card.booster != null)
            {
                if (extensionMissingCompositionsDict.TryGetValue(card.booster.extension.name, out var foundExtension))
                {
                    int boosterIndex = foundExtension.boostersCompositions.FindIndex(boost => boost.name == card.booster.name);
                    foundExtension.boostersCompositions[boosterIndex].rarityPerBoosters[(int)card.rarity].value++;
                    numberOfObtainedCards[card.booster.extension.name]++;
                }
            }
            else
            {
                if (extensionMissingCompositionsDict.TryGetValue(card.extension.name, out var ext))
                {
                    ext.boostersCompositions[0].rarityPerBoosters[(int)card.rarity].value++;
                    numberOfObtainedCards[card.extension.name]++;
                }
            }
        }
        else
        {
            if (missingCards.Contains(card))
            {
                missingCards.Remove(card);
                if (card.booster != null)
                {
                    if (extensionMissingCompositionsDict.TryGetValue(card.booster.extension.name, out var foundExtension))
                    {
                        int boosterIndex = foundExtension.boostersCompositions.FindIndex(boost => boost.name == card.booster.name);
                        foundExtension.boostersCompositions[boosterIndex].rarityPerBoosters[(int)card.rarity].value--;
                        numberOfObtainedCards[card.booster.extension.name]--;
                    }
                }
                else
                {
                    if (extensionMissingCompositionsDict.TryGetValue(card.extension.name, out var ext))
                    {
                        ext.boostersCompositions[0].rarityPerBoosters[(int)card.rarity].value--;
                        numberOfObtainedCards[card.extension.name]--;
                    }
                }
            }
        }

    }

    public void CalculStats(Extension extension, bool secret = true, bool details = true)
    {
        //numberOfCardsInSet = manager.cardsByRarity[(int)RarityManager.Rarity.Common] + manager.cardsByRarity[(int)RarityManager.Rarity.Uncommon] + manager.cardsByRarity[(int)RarityManager.Rarity.Holo] + manager.cardsByRarity[(int)RarityManager.Rarity.Ex];
        //numberOfMissingCardInSet = missingCardsByRarity[(int)RarityManager.Rarity.Common] + missingCardsByRarity[(int)RarityManager.Rarity.Uncommon] + missingCardsByRarity[(int)RarityManager.Rarity.Holo] + missingCardsByRarity[(int)RarityManager.Rarity.Ex];
        var foundExtension = manager.extensions.FirstOrDefault(ext => ext.name == extension.name);
        StatsUI.SaveStats statsExtension = new StatsUI.SaveStats();
        statsExtension.extension = extension;

        int totalCardExtension = 0;
        int totalMissingCardExtension = 0;

        if (foundExtension.cartesCommunes.Count != 0)
        {
            int othersCommonCards = manager.extensionCompositionsDict[extension.name].boostersCompositions[0].rarityPerBoosters[(int)RarityManager.Rarity.Common].value;
            int othersCommonMissingCards = extensionMissingCompositionsDict[extension.name].boostersCompositions[0].rarityPerBoosters[(int)RarityManager.Rarity.Common].value;
            float percentOthersCommon = 1;
            if (othersCommonCards != 0)
            {
                percentOthersCommon = 1 - Mathf.Pow((float)(othersCommonCards - othersCommonMissingCards) / othersCommonCards, 3);
            }
            //Debug.Log("Cartes communes : " + (othersCommonCards - othersCommonMissingCards) + " / " + othersCommonCards);

            int othersUncommonCards = manager.extensionCompositionsDict[extension.name].boostersCompositions[0].rarityPerBoosters[(int)RarityManager.Rarity.Uncommon].value;
            int othersUncommonMissingCards = extensionMissingCompositionsDict[extension.name].boostersCompositions[0].rarityPerBoosters[(int)RarityManager.Rarity.Uncommon].value;
            float percentOthersUncommon = 1;
            if (othersUncommonCards != 0)
            {
                percentOthersUncommon = 1 - ((1 - 0.90f * ((float)othersUncommonMissingCards / othersUncommonCards)) * (1 - 0.60f * ((float)othersUncommonMissingCards / othersUncommonCards)));
            }
            //Debug.Log("Cartes non communes : " + (othersUncommonCards - othersUncommonMissingCards) + " / " + othersUncommonCards);

            int othersHoloCards = manager.extensionCompositionsDict[extension.name].boostersCompositions[0].rarityPerBoosters[(int)RarityManager.Rarity.Holo].value;
            int othersHoloMissingCards = extensionMissingCompositionsDict[extension.name].boostersCompositions[0].rarityPerBoosters[(int)RarityManager.Rarity.Holo].value;
            float percentOthersHolo = 1;
            if (othersHoloCards != 0)
            {
                percentOthersHolo = 1 - ((1 - 0.05f * ((float)othersHoloMissingCards / othersHoloCards)) * (1 - 0.50f * ((float)othersHoloMissingCards / othersHoloCards)));
                //Debug.Log("Cartes holo : " + (othersHoloCards - othersHoloMissingCards) + " / " + othersHoloCards);
            }

            int othersGoldCards = manager.extensionCompositionsDict[extension.name].boostersCompositions[0].rarityPerBoosters[(int)RarityManager.Rarity.Gold].value;
            int othersGoldMissingCards = extensionMissingCompositionsDict[extension.name].boostersCompositions[0].rarityPerBoosters[(int)RarityManager.Rarity.Gold].value;
            float percentOthersGold = 1;
            if (othersGoldCards != 0)
            {
                percentOthersGold = 1 - ((1 - 0.0004f * ((float)othersGoldMissingCards / othersGoldCards)) * (1 - 0.0016f * ((float)othersGoldMissingCards / othersGoldCards)));
                //Debug.Log("Cartes Gold : " + (othersHoloCards - othersHoloMissingCards) + " / " + othersHoloCards);
            }

            float percentOthersTotal;
            int othersTotalMissing = othersCommonMissingCards + othersUncommonMissingCards + othersHoloMissingCards;
            int othersTotalCard = othersCommonCards + othersUncommonCards + othersHoloCards;

            if (secret)
            {
                percentOthersTotal = 1 - ((1 - percentOthersCommon) * (1 - percentOthersUncommon) * (1 - percentOthersHolo) * (1 - percentOthersGold));
                othersTotalMissing += othersGoldMissingCards;
                othersTotalCard += othersGoldCards;
            }
            else
            {
                percentOthersTotal = 1 - ((1 - percentOthersCommon) * (1 - percentOthersUncommon) * (1 - percentOthersHolo));
            }


            statsExtension.extension = extension;

            statsExtension.listFlat.Add(othersCommonMissingCards);
            statsExtension.listFlat.Add(othersUncommonMissingCards);
            statsExtension.listFlat.Add(othersHoloMissingCards);

            statsExtension.listTotal.Add(othersCommonCards);
            statsExtension.listTotal.Add(othersUncommonCards);
            statsExtension.listTotal.Add(othersHoloCards);

            //i=nombre de rareté secret
            for (int i = 0; i < 6; i++)
            {
                statsExtension.listFlat.Add(0);
                statsExtension.listTotal.Add(0);
            }

            if (secret)
            {
                statsExtension.listFlat.Add(othersGoldMissingCards);
                statsExtension.listTotal.Add(othersGoldCards);
            }
            else
            {
                statsExtension.listFlat.Add(0);
                statsExtension.listTotal.Add(0);
            }
            statsExtension.listFlat.Add(othersTotalMissing);
            statsExtension.listTotal.Add(othersTotalCard);

        }
        else
        {
            //i=nombre de raretés
            for (int i = 0; i < 10; i++)
            {
                statsExtension.listFlat.Add(0);
                statsExtension.listTotal.Add(0);
            }
        }
        totalCardExtension += statsExtension.listTotal[statsExtension.listTotal.Count - 1];
        totalMissingCardExtension += statsExtension.listFlat[statsExtension.listFlat.Count - 1];

        foreach (Booster booster in extension.boosters)
        {
            //Calcul des stats
            //Common
            int boosterIndex = extensionMissingCompositionsDict[extension.name].boostersCompositions.FindIndex(boost => boost.name == booster.name);
            int commonCards = manager.extensionCompositionsDict[extension.name].boostersCompositions[boosterIndex].rarityPerBoosters[(int)RarityManager.Rarity.Common].value;

            int boosterMissingIndex = extensionMissingCompositionsDict[extension.name].boostersCompositions.FindIndex(boost => boost.name == booster.name);
            int commonMissing = extensionMissingCompositionsDict[extension.name].boostersCompositions[boosterMissingIndex].rarityPerBoosters[(int)RarityManager.Rarity.Common].value;
            //Debug.Log(booster.name + (commonCards - commonMissing) + " / " + commonCards);

            float percentCommon = 1 - Mathf.Pow((float)(commonCards - commonMissing) / commonCards, 3);
            //Debug.Log("Chance de drop une nouvelle commune dans " + booster.name + " : " + percentCommon * 100 + "%");

            //Uncommun
            int uncommonCards = manager.extensionCompositionsDict[extension.name].boostersCompositions[boosterIndex].rarityPerBoosters[(int)RarityManager.Rarity.Uncommon].value;
            int uncommonMissing = extensionMissingCompositionsDict[extension.name].boostersCompositions[boosterMissingIndex].rarityPerBoosters[(int)RarityManager.Rarity.Uncommon].value;

            float percentUncommon = 1 - ((1 - 0.90f * ((float)uncommonMissing / uncommonCards)) * (1 - 0.60f * ((float)uncommonMissing / uncommonCards)));
            //Debug.Log("Chance de drop une nouvelle uncommon dans : " + booster.name + percentUncommon * 100 + "%");

            //Holo
            int holoCards = manager.extensionCompositionsDict[extension.name].boostersCompositions[boosterIndex].rarityPerBoosters[(int)RarityManager.Rarity.Holo].value;
            int holoMissing = extensionMissingCompositionsDict[extension.name].boostersCompositions[boosterMissingIndex].rarityPerBoosters[(int)RarityManager.Rarity.Holo].value;

            float percentHolo = 1 - ((1 - 0.05f * ((float)holoMissing / holoCards)) * (1 - 0.50f * ((float)holoMissing / holoCards)));
            //Debug.Log("Chance de drop une nouvelle holo dans : " + booster.name + percentHolo * 100 + "%");

            //Ex
            int exCards = manager.extensionCompositionsDict[extension.name].boostersCompositions[boosterIndex].rarityPerBoosters[(int)RarityManager.Rarity.Ex].value;
            int exMissing = extensionMissingCompositionsDict[extension.name].boostersCompositions[boosterMissingIndex].rarityPerBoosters[(int)RarityManager.Rarity.Ex].value;

            float percentEx = 1 - ((1 - 0.016666f * ((float)exMissing / exCards)) * (1 - 0.06664f * ((float)exMissing / exCards)));
            //Debug.Log("Chance de drop une nouvelle Ex dans : " + booster.name + percentEx * 100 + "%");

            //One Star
            int oneStarCards = manager.extensionCompositionsDict[extension.name].boostersCompositions[boosterIndex].rarityPerBoosters[(int)RarityManager.Rarity.OneStar].value;
            int oneStarMissing = extensionMissingCompositionsDict[extension.name].boostersCompositions[boosterMissingIndex].rarityPerBoosters[(int)RarityManager.Rarity.OneStar].value;

            float percentOneStar = 1 - ((1 - 0.02572f * ((float)oneStarMissing / oneStarCards)) * (1 - 0.10288f * ((float)oneStarMissing / oneStarCards)));
            //Debug.Log("Chance de drop une nouvelle One Star dans : " + booster.name + percentEx * 100 + "%");

            //Two Star
            int twoStarsCards = manager.extensionCompositionsDict[extension.name].boostersCompositions[boosterIndex].rarityPerBoosters[(int)RarityManager.Rarity.TwoStars].value;
            int twoStarsMissing = extensionMissingCompositionsDict[extension.name].boostersCompositions[boosterMissingIndex].rarityPerBoosters[(int)RarityManager.Rarity.TwoStars].value;

            float percentTwoStars = 1 - ((1 - 0.005f * ((float)twoStarsMissing / twoStarsCards)) * (1 - 0.02f * ((float)twoStarsMissing / twoStarsCards)));
            //Debug.Log("Chance de drop une nouvelle Two Stars dans : " + booster.name + percentEx * 100 + "%");

            //Three Star
            int immersiveCards = manager.extensionCompositionsDict[extension.name].boostersCompositions[boosterIndex].rarityPerBoosters[(int)RarityManager.Rarity.Immersive].value;
            int immersiveMissing = extensionMissingCompositionsDict[extension.name].boostersCompositions[boosterMissingIndex].rarityPerBoosters[(int)RarityManager.Rarity.Immersive].value;

            float percentImmersive = 1 - ((1 - 0.00222f * ((float)immersiveMissing / immersiveCards)) * (1 - 0.00888f * ((float)immersiveMissing / immersiveCards)));

            //One Shiny
            int oneShinyCards = manager.extensionCompositionsDict[extension.name].boostersCompositions[boosterIndex].rarityPerBoosters[(int)RarityManager.Rarity.OneShiny].value;
            int oneShinyMissing = extensionMissingCompositionsDict[extension.name].boostersCompositions[boosterMissingIndex].rarityPerBoosters[(int)RarityManager.Rarity.OneShiny].value;

            float percentOneShiny = 1 - ((1 - 0.00714f * ((float)oneShinyMissing / oneShinyCards)) * (1 - 0.02857f * ((float)oneShinyMissing / oneShinyCards)));

            //Two Shinys
            int twoShinysCards = manager.extensionCompositionsDict[extension.name].boostersCompositions[boosterIndex].rarityPerBoosters[(int)RarityManager.Rarity.TwoShinys].value;
            int twoShinysMissing = extensionMissingCompositionsDict[extension.name].boostersCompositions[boosterMissingIndex].rarityPerBoosters[(int)RarityManager.Rarity.TwoShinys].value;

            float percentTwoShinys = 1 - ((1 - 0.00333f * ((float)twoShinysMissing / twoShinysCards)) * (1 - 0.01333f * ((float)twoShinysMissing / twoShinysCards)));

            //Gold
            int goldCards = manager.extensionCompositionsDict[extension.name].boostersCompositions[boosterIndex].rarityPerBoosters[(int)RarityManager.Rarity.Gold].value;
            int goldMissing = extensionMissingCompositionsDict[extension.name].boostersCompositions[boosterMissingIndex].rarityPerBoosters[(int)RarityManager.Rarity.Gold].value;

            float percentGold = 1 - ((1 - 0.0004f * ((float)goldMissing / goldCards)) * (1 - 0.0016f * ((float)goldMissing / goldCards)));
            //Debug.Log("Chance de drop une nouvelle Two Stars dans : " + booster.name + percentEx * 100 + "%");


            //Debug.Log(booster.name + (booster.cartes.Count - commonMissing - uncommonMissing - holoMissing - exMissing) + " / " + booster.cartes.Count);
            float percentTotal;
            if (percentCommon == 1)
            {
                percentTotal = 1;
            }
            else
            {
                if (secret)
                {
                    percentTotal = 1 - ((1 - percentCommon) * (1 - percentUncommon) * (1 - percentHolo) * (1 - percentEx) * (1 - percentOneStar) * (1 - percentTwoStars) * (1 - percentImmersive) * (1 - percentOneShiny) * (1 - percentTwoShinys) * (1 - percentGold));
                }
                else
                {
                    percentTotal = 1 - ((1 - percentCommon) * (1 - percentUncommon) * (1 - percentHolo) * (1 - percentEx));
                }
            }

            int totalMissing = commonMissing + uncommonMissing + holoMissing + exMissing;
            if (secret)
            {
                totalMissing += oneStarMissing + twoStarsMissing + immersiveMissing + oneShinyMissing + twoShinysMissing + goldMissing;
            }


            //Debug.Log("Chance de drop une nouvelle carte dans " + booster.name + " " + percentTotal * 100 + "%");

            StatsUI.SaveStats saved = new StatsUI.SaveStats();
            saved.booster = booster;
            saved.listPercent.Add(percentCommon);
            saved.listPercent.Add(percentUncommon);
            saved.listPercent.Add(percentHolo);
            saved.listPercent.Add(percentEx);

            saved.listFlat.Add(commonMissing);
            saved.listFlat.Add(uncommonMissing);
            saved.listFlat.Add(holoMissing);
            saved.listFlat.Add(exMissing);

            saved.listTotal.Add(commonCards);
            saved.listTotal.Add(uncommonCards);
            saved.listTotal.Add(holoCards);
            saved.listTotal.Add(exCards);

            statsExtension.listTotal[(int)RarityManager.Rarity.Common] += commonCards;
            statsExtension.listTotal[(int)RarityManager.Rarity.Uncommon] += uncommonCards;
            statsExtension.listTotal[(int)RarityManager.Rarity.Holo] += holoCards;
            statsExtension.listTotal[(int)RarityManager.Rarity.Ex] += exCards;

            statsExtension.listFlat[(int)RarityManager.Rarity.Common] += commonMissing;
            statsExtension.listFlat[(int)RarityManager.Rarity.Uncommon] += uncommonMissing;
            statsExtension.listFlat[(int)RarityManager.Rarity.Holo] += holoMissing;
            statsExtension.listFlat[(int)RarityManager.Rarity.Ex] += exMissing;

            int totalCard = commonCards + uncommonCards + holoCards + exCards;


            if (secret)
            {
                saved.listPercent.Add(percentOneStar);
                saved.listPercent.Add(percentTwoStars);
                saved.listPercent.Add(percentImmersive);
                saved.listPercent.Add(percentOneShiny);
                saved.listPercent.Add(percentTwoShinys);
                saved.listPercent.Add(percentGold);

                saved.listFlat.Add(oneStarMissing);
                saved.listFlat.Add(twoStarsMissing);
                saved.listFlat.Add(immersiveMissing);
                saved.listFlat.Add(oneShinyMissing);
                saved.listFlat.Add(twoShinysMissing);
                saved.listFlat.Add(goldMissing);

                saved.listTotal.Add(oneStarCards);
                saved.listTotal.Add(twoStarsCards);
                saved.listTotal.Add(immersiveCards);
                saved.listTotal.Add(oneShinyCards);
                saved.listTotal.Add(twoShinysCards);
                saved.listTotal.Add(goldCards);

                statsExtension.listTotal[(int)RarityManager.Rarity.OneStar] += oneStarCards;
                statsExtension.listTotal[(int)RarityManager.Rarity.TwoStars] += twoStarsCards;
                statsExtension.listTotal[(int)RarityManager.Rarity.Immersive] += immersiveCards;
                statsExtension.listTotal[(int)RarityManager.Rarity.OneShiny] += oneShinyCards;
                statsExtension.listTotal[(int)RarityManager.Rarity.TwoShinys] += twoShinysCards;
                statsExtension.listTotal[(int)RarityManager.Rarity.Gold] += goldCards;

                statsExtension.listFlat[(int)RarityManager.Rarity.OneStar] += oneStarMissing;
                statsExtension.listFlat[(int)RarityManager.Rarity.TwoStars] += twoStarsMissing;
                statsExtension.listFlat[(int)RarityManager.Rarity.Immersive] += immersiveMissing;
                statsExtension.listFlat[(int)RarityManager.Rarity.OneShiny] += oneShinyMissing;
                statsExtension.listFlat[(int)RarityManager.Rarity.TwoShinys] += twoShinysMissing;
                statsExtension.listFlat[(int)RarityManager.Rarity.Gold] += goldMissing;


                totalCard += oneStarCards + twoStarsCards + immersiveCards + oneShinyCards + twoShinysCards + goldCards;
            }
            else
            {
                //i= nombre de rareté secret
                for (int i = 0; i < 6; i++)
                {
                    saved.listPercent.Add(0);
                    saved.listFlat.Add(0);
                    saved.listTotal.Add(0);
                }
            }

            totalCardExtension += totalCard;
            totalMissingCardExtension += totalMissing;

            saved.listPercent.Add(percentTotal);
            saved.listFlat.Add(totalMissing);
            saved.listTotal.Add(totalCard);


            statsExtension.listFlat[statsExtension.listFlat.Count - 1] = totalMissingCardExtension;
            statsExtension.listTotal[statsExtension.listFlat.Count - 1] = totalCardExtension;


            StatsUI statsUI = FindAnyObjectByType<StatsUI>();
            statsUI.DisplayStats(saved, statsExtension, details);
        }
    }

    public int GetNumberOfCards(Extension extension)
    {
        var foundExtension = manager.extensions.FirstOrDefault(ext => ext.name == extension.name);
        int totalCards = 0;
        if (foundExtension.cartesCommunes.Count > 0)
        {
            foreach (Carte carte in foundExtension.cartesCommunes)
            {
                if (carte.rarity < RarityManager.Rarity.OneStar)
                {
                    totalCards++;
                }
            }
        }

        foreach (Booster booster in foundExtension.boosters)
        {
            foreach (Carte carte in booster.cartes)
            {
                if (carte.rarity < RarityManager.Rarity.OneStar)
                {
                    totalCards++;
                }
            }
        }

        return totalCards;
    }

    public int GetNumberOfMissingCards(Extension extension)
    {
        int totalMissingCards = 0;
        var foundExtension = manager.extensions.FirstOrDefault(ext => ext.name == extension.name);

        for (int i = 0; i < extensionMissingCompositionsDict[extension.name].boostersCompositions.Count; i++)
        {
            for (int j = 0; j < (int)RarityManager.Rarity.OneStar; j++)
            {
                totalMissingCards += extensionMissingCompositionsDict[extension.name].boostersCompositions[i].rarityPerBoosters[j].value;
            }
        }
        return totalMissingCards;
    }

    public int GetNumberOfSecrets(Extension extension)
    {
        var foundExtension = manager.extensions.FirstOrDefault(ext => ext.name == extension.name);
        int totalCards = 0;
        if (foundExtension.cartesCommunes.Count > 0)
        {
            foreach (Carte carte in foundExtension.cartesCommunes)
            {
                if (carte.rarity >= RarityManager.Rarity.OneStar)
                {
                    totalCards++;
                }
            }
        }

        foreach (Booster booster in foundExtension.boosters)
        {
            foreach (Carte carte in booster.cartes)
            {
                if (carte.rarity >= RarityManager.Rarity.OneStar)
                {
                    totalCards++;
                }
            }
        }

        return totalCards;
    }

    public int GetNumberOfMissingSecrets(Extension extension)
    {
        int totalMissingCards = 0;
        var foundExtension = manager.extensions.FirstOrDefault(ext => ext.name == extension.name);

        for (int i = 0; i < extensionMissingCompositionsDict[extension.name].boostersCompositions.Count; i++)
        {
            for (int j = (int)RarityManager.Rarity.OneStar; j <= (int)RarityManager.Rarity.Gold; j++)
            {
                totalMissingCards += extensionMissingCompositionsDict[extension.name].boostersCompositions[i].rarityPerBoosters[j].value;
            }
        }
        return totalMissingCards;
    }
}
