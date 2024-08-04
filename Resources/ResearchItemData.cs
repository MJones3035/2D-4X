using Godot;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class ResearchItemData : Resource
    {
        [Export] public AllEnums.ResearchTiers tier; // We might not need this now that the research is manually added to the research panel.
        [Export] public AllEnums.Skills skill;
        [Export] public AllEnums.Interests interest;
        [Export] public bool researchedAtStart;
        [Export] public string researchName;
        [Export] public string researchDescription;
        [Export] public Texture2D researchTexture;

        [Export] private int amountOfResearchDone;
        public int AmountOfResearchDone
        {
            get { return amountOfResearchDone; }
            set
            {
                amountOfResearchDone = value;
                if (amountOfResearchDone >= costOfResearch)
                {
                    isResearchDone = true;
                    amountOfResearchDone = costOfResearch;
                }
                else
                {
                    isResearchDone = false;
                }
            }
        }
        [Export] public int costOfResearch;
        [Export] public CountyImprovementData[] countyImprovementDatas;
        public CountyPopulation[] researchers;

        [Export] public bool isResearchDone; // We might not need this because we could just compare the cost vs the amount done.
    }
}