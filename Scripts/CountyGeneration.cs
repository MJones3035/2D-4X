using Godot;
using System.Collections.Generic;
using System.Linq;

namespace PlayerSpace
{
    public partial class CountyGeneration : Node
    {
        // I think we might be able to get rid of these.
        private int perishable;
        private int nonperishable;
        public override void _Ready()
        {
            AssignFactionDataToCountyData();
            GenerateBuildings();
            AssignCountyDataToFaction();
            SubscribeToCountyHeroLists();
            UpdateResources();
            UpdateInitialCountyStorage();
        }

        private static void UpdateResources()
        {
            // Assign a copy of each resource to each county.
            foreach (County county in Globals.Instance.countiesParent.GetChildren().Cast<County>())
            {
                CopyAndAssignResources(county, AllResources.Instance.allResources);
            }
        }

        private static void CopyAndAssignResources(County county, ResourceData[] resources)
        {

            foreach (ResourceData resource in resources)
            {
                if (resource.perishable)
                {
                    AllEnums.CountyResourceType key = resource.countyResourceType;
                    county.countyData.perishableResources[key] = (ResourceData)resource.Duplicate();
                }
                else
                {
                    AllEnums.CountyResourceType key = resource.countyResourceType;
                    county.countyData.nonperishableResources[key] = (ResourceData)resource.Duplicate();
                }
            }

            SetInitialMaxStorage(county, county.countyData.perishableResources);
            SetInitialMaxStorage(county, county.countyData.nonperishableResources);
        }
        private static void SetInitialMaxStorage(County county, Godot.Collections.Dictionary<AllEnums.CountyResourceType, ResourceData> resources)
        {
            foreach (KeyValuePair<AllEnums.CountyResourceType, ResourceData> keyValuePair in resources)
            {
                ResourceData resource = keyValuePair.Value;

                if (resource.perishable)
                {
                    resource.MaxAmount = Globals.Instance.startingPerishableStorage / resources.Count;
                }
                else
                {
                    resource.MaxAmount = Globals.Instance.startingNonperishableStorage / resources.Count;
                }
                GD.Print($"{county.countyData.countyName} - {resource.resourceName}: " +
                        $"{resource.MaxAmount}");
            }
        }

        private static void UpdateInitialCountyStorage()
        {
            foreach (County county in Globals.Instance.countiesParent.GetChildren().Cast<County>())
            {
                county.countyData.perishableStorage = Globals.Instance.startingPerishableStorage;
                county.countyData.nonperishableStorage = Globals.Instance.startingNonperishableStorage;
                GD.Print($"{county.countyData.countyName} has {county.countyData.perishableStorage} perishable storage.");
            }
        }

        private static void SubscribeToCountyHeroLists()
        {
            foreach (County selectCounty in Globals.Instance.countiesParent.GetChildren().Cast<County>())
            {
                selectCounty.countyData.herosInCountyList.ItemAdded += (sender, item) => Globals.Instance.AddToFactionHeroList(item);
            }
        }

        // This is just temporary until we set up random faction generation.
        private static void AssignFactionDataToCountyData()
        {
            // Cowlitz
            County selectCounty = (County)Globals.Instance.countiesParent.GetChild(0);
            //GD.Print("Assigning Faction data: " + Globals.Instance.factionDatas[0].factionName);
            selectCounty.countyData.factionData = Globals.Instance.factionDatas[0];
            //GD.Print("Assigned Faction Data: " + selectCounty.countyData.factionData.factionName);
            // Tillamook
            selectCounty = (County)Globals.Instance.countiesParent.GetChild(1);
            selectCounty.countyData.factionData = Globals.Instance.factionDatas[1];
            // Douglas
            selectCounty = (County)Globals.Instance.countiesParent.GetChild(2);
            selectCounty.countyData.factionData = Globals.Instance.factionDatas[1];
            // Portland
            selectCounty = (County)Globals.Instance.countiesParent.GetChild(3);
            selectCounty.countyData.factionData = Globals.Instance.factionDatas[3];
            // Wasco
            selectCounty = (County)Globals.Instance.countiesParent.GetChild(4);
            selectCounty.countyData.factionData = Globals.Instance.factionDatas[3];
            // Harney
            selectCounty = (County)Globals.Instance.countiesParent.GetChild(5);
            selectCounty.countyData.factionData = Globals.Instance.factionDatas[1];
            // Umatilla
            selectCounty = (County)Globals.Instance.countiesParent.GetChild(6);
            selectCounty.countyData.factionData = Globals.Instance.factionDatas[2];
        }


        private static void AssignCountyDataToFaction()
        {
            // This goes through every county and adds itself to the faction data that is already assigned to the county.
            foreach (County selectCounty in Globals.Instance.countiesParent.GetChildren().Cast<County>())
            {
                selectCounty.countyData.factionData.countiesFactionOwns.Add(selectCounty.countyData);
                //GD.Print($"Faction: {selectCounty.countyData.factionData.factionName} {selectCounty.countyData.countyName}");
            }
        }

        private static void GenerateBuildings()
        {
            foreach (County selectCounty in Globals.Instance.countiesParent.GetChildren().Cast<County>())
            {
                //GD.Print("County Generation: " + selectCounty.Name);
                //GD.Print("County Data: " + selectCounty.countyData.countyName);
                //GD.Print("Faction Data: " + selectCounty.countyData.factionData.factionName);

                foreach (ResearchItemData researchItemData in selectCounty.countyData.factionData.researchItems)
                {
                    if (researchItemData.isResearchDone == true && researchItemData.countyImprovementDatas.Length > 0)
                    {
                        foreach (CountyImprovementData countyImprovementData in researchItemData.countyImprovementDatas)
                        {
                            selectCounty.countyData.allCountyImprovements
                                .Add((CountyImprovementData)countyImprovementData.Duplicate());
                            //GD.Print($"{selectCounty.countyData.countyName} improvement: {countyImprovementData.improvementName}");
                        }
                    }
                }
            }
        }
    }
}