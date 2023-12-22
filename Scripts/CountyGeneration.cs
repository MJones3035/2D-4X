using Godot;
using System;

namespace PlayerSpace
{
	public partial class CountyGeneration : Node
	{
		public override void _Ready()
		{
			GenerateBuildings();
			AssignCountyDataToFaction();
		}

        private void AssignCountyDataToFaction()
        {
            foreach (Node node in Globals.Instance.countiesParent.GetChildren())
			{
                SelectCounty selectCounty = (SelectCounty)node;
				selectCounty.countyData.factionData.countiesFactionOwns.Add(selectCounty.countyData);
            }
        }

        private void GenerateBuildings()
        {
            foreach(Node node in Globals.Instance.countiesParent.GetChildren())
			{
				SelectCounty selectCounty = (SelectCounty)node;
				/*
				GD.Print("County Generation: " + selectCounty.Name);
				GD.Print("County Data: " + selectCounty.countyData.countyName);
				GD.Print("Faction Data: " + selectCounty.countyData.factionData.factionName);
				*/
				foreach(ResearchItemData researchItemData in selectCounty.countyData.factionData.researchItems)
				{
					if(researchItemData.countyImprovementData != null)
					{
						selectCounty.countyData.allCountyImprovements.Add(researchItemData.countyImprovementData);
						//GD.Print($"{selectCounty.countyData.countyName} improvement: {researchItemData.countyImprovementData.improvementName}");
					}
				}
			}
        }
    }
}