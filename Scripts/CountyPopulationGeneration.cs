using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

namespace PlayerSpace
{
    public partial class CountyPopulationGeneration : Node
    {
        private readonly Random random = new();

        public Node2D countiesParent;
        private SelectCounty selectCounty;
        private CountyData countyData;

        private string firstName;
        private string lastName;
        private bool isMale;
        private int age;
        private bool isFactionLeader = false;
        private bool leaderOfPeoplePerk = false;

        public override void _Ready()
        {
            CallDeferred("CreatePopulation");
        }
        private void CreatePopulation()
        {
            countiesParent = Globals.Instance.countiesParent;
            // Create various county specific data.
            for (int i = 0; i < countiesParent.GetChildCount(); i++)
            {
                selectCounty = (SelectCounty)countiesParent.GetChild(i);
                countyData = selectCounty.countyData;
                countyData.Population = 0;

                // Generate the general population for the player Capitals.
                if (countyData.isPlayerCapital == true || countyData.isAICapital)
                {
                    // Generate Hero Population
                    GeneratePopulation(true, Globals.Instance.heroPopulation);
                    countyData.Population += countyData.heroCountyPopulation.Count;

                    // Generate Normal Population
                    GeneratePopulation(false, Globals.Instance.totalCapitolPop);
                    countyData.Population += countyData.countyPopulation.Count;
                    countyData.IdleWorkers = countyData.Population;
                }
                else
                {
                    // Generate Hero Population
                    GeneratePopulation(true, Globals.Instance.heroPopulation);
                    countyData.Population += countyData.heroCountyPopulation.Count;

                    // Generate Normal Population
                    int normalPopulation = random.Next(Globals.Instance.minimumCountyPop, Globals.Instance.maximumCountyPop);
                    GeneratePopulation(false, normalPopulation);
                    countyData.Population += countyData.countyPopulation.Count;
                    countyData.IdleWorkers = countyData.Population;
                }
            }
        }

        private void GeneratePopulation(bool hero, int totalPopulation)
        {
            for (int i = 0; i < totalPopulation; i++)
            {
                // Generates Persons Last Name
                List<string> lastNames = Globals.Instance.lastNames;
                List<string> femaleNames = Globals.Instance.femaleNames;
                List<string> maleNames = Globals.Instance.maleNames;

                int randomLastNameNumber = random.Next(0, lastNames.Count);
                lastName = lastNames[randomLastNameNumber];

                // Determine the persons sex and first name
                int randomSexNumber = random.Next(0, 2);
                int randomFemaleNameNumber = random.Next(0, femaleNames.Count);
                int randomMaleNameNumber = random.Next(0, maleNames.Count);

                if (randomSexNumber == 0)
                {
                    isMale = true;
                    firstName = maleNames[randomMaleNameNumber];
                }
                else
                {
                    isMale = false;
                    firstName = femaleNames[randomFemaleNameNumber];
                }

                // Determine the person's age.
                age = random.Next(18, 61);

                // Generate random skill level for each population.
                int constructionSkill = random.Next(20, 81);

                if (hero == false)
                {
                    // This adds to the Godot Dictionary a new person.
                    countyData.countyPopulation.Add(i, new CountyPopulation(firstName, lastName, isMale, age, false,
                        false, false, false, constructionSkill, AllText.Jobs.IDLE, AllText.Jobs.IDLE, false));

                    CountyPopulation person = countyData.countyPopulation[i];
                    GD.Print($"Name: {countyData.countyPopulation[i].firstName} {countyData.countyPopulation[i].lastName} " +
                    $"Age: {person.age} isMale: {person.isMale} Leader of People: {person.leaderOfPeoplePerk} " +
                    $"Faction Leader: {person.isFactionLeader}");
                }
                else
                {
                    // This adds to a C# list.
                    countyData.heroCountyPopulation.Add(new CountyPopulation(firstName, lastName, isMale, age, true,
                        false, false, true, constructionSkill, AllText.Jobs.IDLE, AllText.Jobs.IDLE, false));

                    CountyPopulation heroPerson = countyData.heroCountyPopulation[i];
                    GD.Print($"Hero Name: {heroPerson.firstName} {heroPerson.lastName} " +
                    $"Age: {heroPerson.age} isMale: {heroPerson.isMale} Leader of People: {heroPerson.leaderOfPeoplePerk} " +
                    $"Faction Leader: {heroPerson.isFactionLeader}");
                }
            }

        }
    }
}

/*
for(int i = 0; i < countiesParent.GetChildCount();  i++)
{
SelectCounty selectCounty = (SelectCounty)countiesParent.GetChild(i);
CountyData countyData = selectCounty.countyData;
countyData.countyPopulation.Add(0, new CountyPopulation("Fred", "Dickface", true, 25, true, false, false, true, 50, "None", "None", false));
GD.Print($"First Name: {countyData.countyPopulation[0].firstName} Last Name: {countyData.countyPopulation[0].lastName}");
}
*/