using Godot;

namespace PlayerSpace
{
    public class FactionAI
    {
        // This is very primative logic.  Pretty useless.
        public void AssignResearch(FactionData factionData)
        {
            foreach (CountyPopulation countyPopulation in factionData.allHeroesList)
            {
                // Temporarily assign all heroes to research Researching.
                if(countyPopulation.activity == AllEnums.Activities.Idle)
                {
                    if (countyPopulation.factionData != Globals.Instance.playerFactionData)
                    {
                        countyPopulation.currentResearchItemData = factionData.researchItems[2];
                        //GD.Print($"{countyPopulation.firstName} is researching {countyPopulation.CurrentResearchItemData.researchName}");
                    }
                }
            }
        }
    }
}