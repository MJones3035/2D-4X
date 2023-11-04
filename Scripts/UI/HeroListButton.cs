using Godot;

namespace PlayerSpace
{
    public partial class HeroListButton : PanelContainer
    {
        public CountyPopulation countyPopulation;
        private Node2D heroSpawnParent;
        
        private void HeroButton()
        {
            Globals.Instance.selectedCountyPopulation = countyPopulation;
            CountyInfoControl.Instance.populationDescriptionMarginContainer.Show();
            if(CountyInfoControl.Instance.populationDescriptionMarginContainer.Visible == true)
            {
                PopulationDescriptionUIElement.Instance.UpdateDescriptionInfo();
            }
            CountyInfoControl.Instance.populationListMarginContainer.Hide();
        }

        private void SpawnHeroCheckBox(bool toggleOn)
        {
            if(toggleOn == true && countyPopulation.isSpawned == false)
            {
                SelectCounty selectCounty = (SelectCounty)Globals.Instance.selectedCounty;
                heroSpawnParent = selectCounty.heroSpawn;
                CharacterBody2D spawnedHero = (CharacterBody2D)Globals.Instance.heroToken.Instantiate();
                heroSpawnParent.AddChild(spawnedHero);
                countyPopulation.isSpawned = true;
                SelectToken selectToken = (SelectToken)spawnedHero;
                selectToken.countyPopulation = countyPopulation;
                
            }
        }
    }
}