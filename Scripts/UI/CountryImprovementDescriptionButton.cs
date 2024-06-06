using Godot;

namespace PlayerSpace
{
	public partial class CountryImprovementDescriptionButton : Control
	{
		public CountyImprovementData countyImprovementData;

		[Export] private TextureRect improvementTexture;
		[Export] public Label improvementNameLabel;
		[Export] private Label improvementDescriptionLabel;
		[Export] private Label improvementInfluenceCostLabel;
		[Export] private Label improvementAmountOfConstructionLabel;
		[Export] private Label improvementMaxBuildersLabel;

		[Export] public Label underContructionLabel;

		[Export] private Button buildingButton;

		public override void _Ready()
		{
			CallDeferred("UpdatePossibleBuildingLabels");
		}

		public void UpdatePossibleBuildingLabels()
		{
			Banker banker = new();
			improvementTexture.Texture = countyImprovementData.improvementTexture;
			improvementNameLabel.Text = countyImprovementData.improvementName;
			improvementDescriptionLabel.Text = countyImprovementData.improvementDescription;
			improvementInfluenceCostLabel.Text = $"Influence Cost: {countyImprovementData.influenceCost}";
			if (ResearchDescriptionPanel.Instance.Visible == true)
			{
				buildingButton.Disabled = true;
                improvementMaxBuildersLabel.Text = $"{countyImprovementData.maxBuilders} Builders";
                improvementAmountOfConstructionLabel.Text = $"{countyImprovementData.maxAmountOfConstruction} Amount of Contruction";
                return;
			}
			else if(CountyImprovementsControl.Instance.Visible == true)
			{
				if (banker.CheckBuildingCost(Globals.Instance.SelectedLeftClickCounty.countyData.factionData, countyImprovementData) == false)
				{
					buildingButton.Disabled = true;
				}
				if (countyImprovementData.underConstruction == true || countyImprovementData.isBuilt == true)
				{
					buildingButton.Disabled = true;
				}
				if (countyImprovementData.underConstruction != true)
				{
					improvementAmountOfConstructionLabel.Text = $"Amount of construction: {countyImprovementData.maxAmountOfConstruction}";
					improvementMaxBuildersLabel.Text = $"Max builders: {countyImprovementData.maxBuilders}";
				}
				else
				{
					improvementInfluenceCostLabel.Hide();
					underContructionLabel.Show();
					improvementAmountOfConstructionLabel.Text = $"{countyImprovementData.currentAmountOfConstruction}/{countyImprovementData.maxAmountOfConstruction} Amount of Contruction";
					improvementMaxBuildersLabel.Text = $"{countyImprovementData.currentBuilders}/{countyImprovementData.maxBuilders} Builders";
				}
				if (countyImprovementData.isBuilt == true)
				{
					improvementMaxBuildersLabel.Text = $"{countyImprovementData.currentWorkers}/{countyImprovementData.maxWorkers} Workers";
					improvementAmountOfConstructionLabel.Hide();
					improvementInfluenceCostLabel.Hide();
				}
			}
        }
		
        private void BuildingButton()
		{
			//GD.Print("You have pressed the building button.");
			CountyImprovementsControl.Instance.buildConfirmationDialog.Visible = true;
			Globals.Instance.selectedPossibleBuildingControl = this;
		}

        /*
        public void UpdatePossibleBuildingLabels()
        {
            improvementNameLabel.Text = countyImprovementData.improvementName;
            improvementDescriptionLabel.Text = countyImprovementData.improvementDescription;
            improvementInfluenceCostLabel.Text = $"Influence Cost: {countyImprovementData.influenceCost}";

            switch (Banker.Instance.CheckBuildingCost(Globals.Instance.selectedCountyData, countyImprovementData)
				, countyImprovementData.isBeingBuilt, countyImprovementData.isBuilt)
            {
                case (false, _, _):
                    buildingButton.Disabled = true;
                    break;

                case (_, true, _):
                    buildingButton.Disabled = true;
                    break;

                case (_, _, true):
                    improvementMaxBuildersLabel.Text = $"{countyImprovementData.currentWorkers}/{countyImprovementData.maxWorkers} Workers";
                    improvementAmountOfConstructionLabel.Hide();
                    break;

                case (_, false, _):
                    improvementAmountOfConstructionLabel.Text = $"Amount of work: {countyImprovementData.maxAmountOfConstruction}";
                    improvementMaxBuildersLabel.Text = $"Max Workers: {countyImprovementData.maxBuilders}";
                    break;
				
                case (_, _, _):
                    improvementInfluenceCostLabel.Hide();
                    underContructionLabel.Show();
                    improvementAmountOfConstructionLabel.Text = $"{countyImprovementData.currentAmountOfConstruction}/{countyImprovementData.maxAmountOfConstruction} Amount of Construction";
                    improvementMaxBuildersLabel.Text = $"{countyImprovementData.currentBuilders}/{countyImprovementData.maxBuilders} Builders";
                    break;
				
            }
        }
		*/
    }
}