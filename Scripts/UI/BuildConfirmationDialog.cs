using Godot;

namespace PlayerSpace
{
    public partial class BuildConfirmationDialog : ConfirmationDialog
    {
        private void OnVisibilityChanged()
        {
            CallDeferred("UpdateConfirmatioDialog");
        }

        private void UpdateConfirmatioDialog()
        {
            DialogText = AllText.DialogTitles.BUILDINGBUILDINGS 
                + Globals.Instance.selectedPossibleBuildingControl.countyImprovementData.improvementName;       
        }

        private void YesButton()
        {
            //GD.Print("Yes was pressed.");

            CountyImprovementData countyImprovementData = Globals.Instance.selectedPossibleBuildingControl.countyImprovementData;
            
            Banker.Instance.BuildImprovement(Globals.Instance.selectedLeftClickCounty.countyData, countyImprovementData);
            Banker.Instance.ChargeForBuilding(Globals.Instance.playerFactionData
                , Globals.Instance.selectedPossibleBuildingControl.countyImprovementData);

            Globals.Instance.selectedPossibleBuildingControl.UpdatePossibleBuildingLabels();
            CountyImprovementsControl.Instance.GenerateCountyImprovementButtons();
            Hide();
        }

        
    }
}