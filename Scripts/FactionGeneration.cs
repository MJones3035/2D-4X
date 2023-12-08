using Godot;
using System.Collections.Generic;

namespace PlayerSpace
{
    public partial class FactionGeneration : Node
    {
        private string factionDataPath = "res://Resources/Factions/";

        [Export] private CountyImprovementData[] countyImprovement;

        public override void _Ready()
        {
            //Instance = this;

            GetFactionsFromDisk();
        }

        private void GetFactionsFromDisk()
        {
            
            using var directory = DirAccess.Open(factionDataPath);
            if (directory.DirExists(factionDataPath))
            {
                directory.ListDirBegin();
                string[] fileNames = directory.GetFiles();
                for (int i = 0; i < fileNames.Length; i++)
                {
                    var factionData = ResourceLoader.Load<FactionData>(factionDataPath + fileNames[i]);
                    Globals.Instance.factions.Add(factionData);
                    GD.Print($"Player? {Globals.Instance.factions[i].isPlayer} and Faction Name? {Globals.Instance.factions[i].factionName}");
                    if (Globals.Instance.factions[i].isPlayer == true)
                    {
                        Globals.Instance.playerFactionData = factionData;
                        GD.Print("Player Faction: " + Globals.Instance.playerFactionData.factionName);
                    }

                    AddStartingResearch(factionData);
                }
            }
            else
            {
                GD.Print("You are so fucked.  This directory doesn't exist: " + factionDataPath);
            }
        }

        private void AddStartingResearch(FactionData factionData)
        {
            // Let's turn this into an array or some shit at some point so we don't have manually add everything, we could just do a foreach loop.
            factionData.researchItems.Add(new ResearchItemData(AllText.BuildingName.FISHERSSHACK, AllText.Descriptions.FISHERSSHACK, countyImprovement[0], true));
            factionData.researchItems.Add(new ResearchItemData(AllText.BuildingName.FORESTERSSHACK, AllText.Descriptions.FORESTERSSHACK, countyImprovement[1], true));
            factionData.researchItems.Add(new ResearchItemData(AllText.BuildingName.GARDENERSSHACK, AllText.Descriptions.GARDENERSSHACK, countyImprovement[2], true));
            
        }
    }
}