using Godot;
using System;

namespace PlayerSpace
{
    public class TokenSpawner
    {
        public CountyPopulation Spawn(SelectCounty selectCounty, CountyPopulation countyPopulation)
        {
            // Spawning the token.
            Node2D tokenSpawnParent = selectCounty.heroSpawn;
            SelectToken spawnedToken = (SelectToken)Globals.Instance.heroToken.Instantiate();
            tokenSpawnParent.AddChild(spawnedToken);

            spawnedToken.countyPopulation = countyPopulation;

            AllTokenTextures.Instance.AssignTokenTextures(spawnedToken);

            countyPopulation.token = spawnedToken;
            countyPopulation.location = selectCounty.countyData.countyId;
            spawnedToken.Name = $"{countyPopulation.firstName} {countyPopulation.lastName}";

            // Update the token's name label
            spawnedToken.tokenNameLabel.Text = $"{countyPopulation.firstName} {countyPopulation.lastName}";

            // Spawning the Spawned Token Button
            SpawnedTokenButton spawnedTokenButton = (SpawnedTokenButton)Globals.Instance.spawnedTokenButton.Instantiate();
            if (countyPopulation.IsArmyLeader == false)
            {
                selectCounty.heroesHBox.AddChild(spawnedTokenButton);
                selectCounty.heroesHBox.Show();
            }
            else
            {
                selectCounty.armiesHBox.AddChild(spawnedTokenButton);
                selectCounty.armiesHBox.Show();
            }
            spawnedTokenButton.countyPopulation = countyPopulation;

            selectCounty.countyData.spawnTokenButtons.Add(spawnedTokenButton);

            // The token needs to keep track of this button.
            spawnedToken.spawnedTokenButton = spawnedTokenButton;

            // Add separators depending on if there are more then 1 hero or army.
            /*
            if (selectCounty.heroesHBox.GetChildren().Count > 1)
            {
                selectCounty.heroTokensControl.heroSeparator.Show();
            }

            if (selectCounty.armiesHBox.GetChildren().Count > 1)
            {
                selectCounty.heroTokensControl.armySeparator.Show();
            }
            */

            // This is at the bottom just in case the Getter Setter is fired to fast.
            // This should probably be changed into a public method somewhere.

            DecidedIfSelected(selectCounty, spawnedToken);

            spawnedTokenButton.UpdateTokenTextures(); // This has to be below the countyPopulation assignment.

            return countyPopulation;
        }

        private void DecidedIfSelected(SelectCounty selectCounty, SelectToken spawnedToken)
        {
            GD.Print($"{selectCounty.countyData.factionData.factionName} vs {Globals.Instance.playerFactionData.factionName}");
            if(selectCounty.countyData.factionData == Globals.Instance.playerFactionData)
            {
                spawnedToken.IsSelected = true;
                GD.Print("Spawned Token Button Token's Name: " + spawnedToken.countyPopulation.firstName + spawnedToken.IsSelected);
            }
        }
    }
}