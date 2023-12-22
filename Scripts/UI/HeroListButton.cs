using Godot;

namespace PlayerSpace
{
    public partial class HeroListButton : PanelContainer
    {
        public CountyPopulation countyPopulation;
        private HeroStacker heroSpawnParent;

        [Export] public TextureRect factionLeaderTextureRect;
        [Export] public TextureRect aideTextureRect;
        [Export] public TextureRect armyLeaderTextureRect;
        [Export] public Label heroNameLabel;
        [Export] public CheckButton spawnHeroButton;
        private void HeroButton()
        {
            Globals.Instance.selectedCountyPopulation = countyPopulation;
            CountyInfoControl.Instance.populationDescriptionControl.Show();
            if (CountyInfoControl.Instance.populationDescriptionControl.Visible == true)
            {
                PopulationDescriptionControl.Instance.UpdateDescriptionInfo();
            }
            CountyInfoControl.Instance.populationListMarginContainer.Hide();
        }

        private void SpawnHeroCheckBox(bool toggleOn)
        {
            if (toggleOn == true && countyPopulation.token == null)
            {
                SelectCounty selectCounty = Globals.Instance.selectedSelectCounty;
                heroSpawnParent = selectCounty.heroSpawn;
                CharacterBody2D spawnedHero = (CharacterBody2D)Globals.Instance.heroToken.Instantiate();
                heroSpawnParent.AddChild(spawnedHero);
                countyPopulation.token = spawnedHero;
                SelectToken selectToken = (SelectToken)spawnedHero;
                selectToken.countyPopulation = countyPopulation;
                selectToken.countyPopulation.location = selectCounty.countyData.countyID;
                spawnedHero.Name = $"{selectToken.countyPopulation.firstName} {selectToken.countyPopulation.lastName}";

                // Update the token's name label
                selectToken.tokenNameLabel.Text = $"{countyPopulation.firstName} {countyPopulation.lastName}";

                // Assign to Currently Selected Hero so it is ready to be moved.
                Globals.Instance.CurrentlySelectedToken = selectToken;
                // Add heroToken to counties spawned hero list
                selectCounty.heroSpawn.spawnedTokenList.Insert(0, selectToken);
            }
        }
    }
}