using Godot;
using System.Collections.Generic;

namespace PlayerSpace
{
    public partial class County : Node2D
    {
        [Export] public CountyData countyData;

        [ExportGroup("Attached Nodes")]
        [Export] public Sprite2D maskSprite;
        [Export] public Sprite2D countySprite;
        [Export] public Sprite2D capitalSprite;
        [Export] public Node2D countyOverlayNode2D;
        [Export] public Node2D heroSpawn;
        [Export] public BattleControl battleControl;

        [Export] public HBoxContainer armiesHBox;
        [Export] public HBoxContainer heroesHBox;

        private SelectToken selectToken;

        public List<County> neighborCounties = [];

        public override void _Ready()
        {
            countyData.countyNode = this; // Figure out why I did this.  I bet you this could be removed.
            if (GetTree().CurrentScene.SceneFilePath == "res://Scenes/Main.tscn")
            {
                Clock.Instance.SetDay += EndOfDay;
                Clock.Instance.SetDay += DayStart;
            }
        }

        private void EndOfDay()
        {
            CountyAI countyAI = new();

            GD.Print("County Hour Zero.");
            countyAI.DecideBuildingCountyImprovements(this);
            PopulationAI.WorkDayOverForPopulation(this);
            PopulationAI.IsThereEnoughFood(countyData); // This is a terrible name for this method.
            // Population uses other resources besides food.

            // Update the Top Bar with the resources used yesterday.
            TopBarControl.Instance.UpdateResourcesUsedYesterday();
            // This is busted until we add people to the County Improvement List.
            CountyAI.CheckIfCountyImprovementsAreDone(countyData);

            // Have population uses resources.
        }

        private void DayStart()
        {
            PopulationAI populationAI = new();
            // This is the one with the dumb name.
            populationAI.CheckForWork(this); // We need to add the population to the new peopleAtCountyImprovementList.
        }
        private void OnTreeExit()
        {
            Clock.Instance.SetDay -= EndOfDay;
            Clock.Instance.SetDay -= DayStart;
        }
    }
}

