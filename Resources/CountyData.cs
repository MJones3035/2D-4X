using Godot;
using System;
using System.Collections.Generic;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class CountyData : Resource
    {
        [ExportGroup("MapEditor")]
        public County countyNode; // See if we can get rid of this somehow.
        [Export] public Color color;
        public Vector2I startMaskPosition; // I think this is the local position....
        [Export] public Vector2I countyOverlayLocalPosition;

        [ExportGroup("County other somethings")]
        [Export] public int countyId;
        [Export] public string countyName;
        [Export] public bool isPlayerCapital; // We need to differentiate between player choosen capitals and AI capitals for generation after player creation.
        [Export] public bool isAICapital;
        //[Export] public AllEnums.Factions faction;
        [Export] public FactionData factionData;

        [Export] public AllEnums.Province province;
        [Export] public AllEnums.Terrain biomePrimary;
        [Export] public AllEnums.Terrain biomeSecondary;
        [Export] public AllEnums.Terrain biomeTertiary;

        public Globals.ListWithNotify<CountyPopulation> countyPopulationList = [];
        public Globals.ListWithNotify<CountyPopulation> herosInCountyList = [];
        public Globals.ListWithNotify<CountyPopulation> armiesInCountyList = [];
        public Globals.ListWithNotify<CountyPopulation> visitingHeroList = [];
        public Globals.ListWithNotify<CountyPopulation> visitingArmyList = [];
        public Globals.ListWithNotify<CountyPopulation> deadPeopleList = [];

        public List<CountyPopulation> possibleWorkers = []; // List of all the idle, helpful and loyal workers for that day.
        public List<CountyPopulation> workersToRemoveFromPossibleWorkers = []; // List to collect county populations to be removed from the possibleWorkers.

        public List<Button> spawnedTokenButtons = [];

        public List<CountyImprovementData> allCountyImprovements = []; // This includes all county improvements, even possible ones.
        public List<CountyImprovementData> underConstructionCountyImprovements = [];
        public List<CountyImprovementData> completedCountyImprovements = [];
        public List<Battle> battles = [];

        public int population = 0;
        [Export] public int perishableStorage;
        [Export] public int nonperishableStorage;

        [Export] public int scavengableScrap; // This the total a county has available to scavenge.
        [Export] public int scavengableFood; // This the total a county has available to scavenge.

        [Export] public Godot.Collections.Dictionary<AllEnums.CountyResourceType, CountyResourceData> countyResources = [];
        [Export] public Godot.Collections.Dictionary<AllEnums.CountyResourceType, CountyResourceData> yesterdaysCountyResources = [];
        [Export] public Godot.Collections.Dictionary<AllEnums.CountyResourceType, CountyResourceData> amountUsedCountyResources = [];

        // These are used just to pass some data around.  Probably I should find a better way to do this.
        public Texture2D maskTexture;
        public Texture2D mapTexture;

        // We will have to see if this is still used.
        public event Action<bool> CountySelected;

        private bool selected = false;
        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                if (selected)
                {
                    OnCountySelected(true);
                }
                else
                {
                    OnCountySelected(false);
                }
            }
        }

        private void OnCountySelected(bool isSelected)
        {
            CountySelected?.Invoke(isSelected);
        }

        int idleWorkers;

        public int IdleWorkers
        {
            get { return idleWorkers; }
            set
            {
                idleWorkers = value;
                //GD.Print("Idle workers: " + idleWorkers);
                if (Globals.Instance.SelectedLeftClickCounty?.countyData == this)
                {
                    CountyInfoControl.Instance.UpdateIdleWorkersLabel();
                }
            }
        }

        public void CheckIfCountyImprovementsAreDone()
        {
            List<CountyImprovementData> completedImprovments = [];
            completedImprovments.Clear();
            foreach (CountyImprovementData countyImprovementData in underConstructionCountyImprovements)
            {
                // If the county improvement is done, make everyone working on it idle.
                // Set their current work to null.
                if (countyImprovementData.CheckIfCountyInprovementDone())
                {
                    foreach (CountyPopulation countyPopulation in countyImprovementData.countyPopulationAtImprovement)
                    {
                        countyPopulation.UpdateActivity(AllEnums.Activities.Idle);
                        countyPopulation.UpdateCurrentCountyImprovement(null);
                    }
                    // Set countyImprovement status to Complete
                    countyImprovementData.SetCountyImprovementComplete();
                    // Clear the people on the county improvement list.
                    countyImprovementData.countyPopulationAtImprovement.Clear();
                    completedImprovments.Add(countyImprovementData);
                }
            }
            // Move the county improvement to the correct list and remove it from the old list.
            MoveCountyImprovementToCompletedList(completedImprovments);
        }
        public void CheckForPreferredWork()
        {
            //GD.Print($"{county.countyData.countyName}: Checking for Preferred Work!");
            foreach (CountyImprovementData countyImprovementData in completedCountyImprovements)
            {
                foreach (CountyPopulation countyPopulation in possibleWorkers)
                {
                    // If they have the preferred skill, they are added to the county improvement
                    // and marked for removal from the possibleWorkers list.
                    if (countyPopulation.preferredSkill.skill == countyImprovementData.workSkill)
                    {
                        if (countyImprovementData.countyPopulationAtImprovement.Count
                            < countyImprovementData.maxWorkers)
                        {
                            countyPopulation.UpdateActivity(AllEnums.Activities.Work);
                            UpdateWorkLocation(countyPopulation, countyImprovementData);
                            GD.Print($"{countyPopulation.firstName} preferred work is " +
                                $"{countyPopulation.preferredSkill.skillName} and they are " +
                                $"{countyPopulation.GetActivityName()} at " +
                                $"{countyPopulation.CurrentCountyImprovment.improvementName}");
                        }
                    }
                }
                RemoveWorkersFromPossibleWorkers();
            }
        }

        public void CheckForAnyWork()
        {
            foreach (CountyImprovementData countyImprovementData in completedCountyImprovements)
            {
                foreach (CountyPopulation countyPopulation in possibleWorkers)
                {
                    if (countyImprovementData.countyPopulationAtImprovement.Count 
                        < countyImprovementData.maxWorkers)
                    {
                        countyPopulation.UpdateActivity(AllEnums.Activities.Work);
                        UpdateWorkLocation(countyPopulation, countyImprovementData);
                    }
                }
                RemoveWorkersFromPossibleWorkers();
            }
        }

        private void UpdateWorkLocation(CountyPopulation countyPopulation, CountyImprovementData countyImprovementData)
        {
            // This same thing is done multiple times.  We should make it its own method.
            countyPopulation.UpdateCurrentCountyImprovement(countyImprovementData);
            countyImprovementData.countyPopulationAtImprovement.Add(countyPopulation);
            workersToRemoveFromPossibleWorkers.Add(countyPopulation);
        }
        public void CountIdleWorkers()
        {
            int idleWorkers = 0;
            foreach (CountyPopulation person in countyPopulationList)
            {
                if (person.activity == AllEnums.Activities.Idle)
                {
                    idleWorkers++;
                }
            }
            IdleWorkers = idleWorkers;
        }

        // If there isn't enough remnants then have the idle people start scavenging.
        public void CheckForScavengingRemnants()
        {
            //GD.Print($"{county.countyData.countyName} Amount of remnants: " + county.countyData.resources[AllEnums.CountyResourceType.Remnants].amount);
            if (EnounghStored(countyResources[AllEnums.CountyResourceType.Remnants].amount, Globals.Instance.remnantsBeforeScavenge) == false)
            {
                foreach (CountyPopulation countyPopulation in possibleWorkers)
                {
                    countyPopulation.UpdateActivity(AllEnums.Activities.Scavenge);
                    workersToRemoveFromPossibleWorkers.Add(countyPopulation);
                }
                RemoveWorkersFromPossibleWorkers();
            }
        }

        public int CountFactionResourceOfType(AllEnums.FactionResourceType resourceType)
        {
            int amount = 0;
            foreach (CountyResourceData resourceData in countyResources.Values)
            {
                if (resourceData.factionResourceType == resourceType)
                {
                    amount += resourceData.amount;
                    //GD.Print($"{countyData.countyName} is counting food: {resourceData.name} {resourceData.amount}");
                }
            }
            return amount;
        }

        public int CountUsedFactionResourceOfType(AllEnums.FactionResourceType resourceType)
        {
            int amount = 0;
            foreach (CountyResourceData resourceData in amountUsedCountyResources.Values)
            {
                if (resourceData.factionResourceType == resourceType)
                {
                    amount += resourceData.amount;
                    //GD.Print($"{countyData.countyName} is counting food: {resourceData.name} {resourceData.amount}");
                }
            }
            return amount;
        }

        // If there isn't enough food then have the idle people start scavenging.
        public void CheckForScavengingFood()
        {
            int amountOfFood = CountFactionResourceOfType(AllEnums.FactionResourceType.Food);
            //GD.Print($"{county.countyData.countyName} Amount of food: " + amountOfFood);
            if (EnounghStored(amountOfFood, Globals.Instance.foodBeforeScavenge) == false)
            {
                foreach (CountyPopulation countyPopulation in possibleWorkers)
                {
                    countyPopulation.UpdateActivity(AllEnums.Activities.Scavenge);
                    workersToRemoveFromPossibleWorkers.Add(countyPopulation);
                }
                RemoveWorkersFromPossibleWorkers();
            }
        }

        private bool EnounghStored(int amountOfStored, int resourceBeforeScavenge)
        {
            if (amountOfStored < resourceBeforeScavenge)
            {
                return false;
            }
            return true;
        }
        public void CheckForConstruction()
        {
            foreach (CountyImprovementData countyImprovementData in underConstructionCountyImprovements)
            {
                foreach (CountyPopulation countyPopulation in possibleWorkers)
                {
                    if (countyImprovementData.countyPopulationAtImprovement.Count 
                        < countyImprovementData.maxBuilders)
                    {
                        countyPopulation.UpdateActivity(AllEnums.Activities.Build);
                        UpdateWorkLocation(countyPopulation, countyImprovementData);
                    }
                }
                RemoveWorkersFromPossibleWorkers();
            }
        }



        private void RemoveWorkersFromPossibleWorkers()
        {
            // Remove the collected items from the possibleWorkers list
            foreach (CountyPopulation countyPopulation in workersToRemoveFromPossibleWorkers)
            {
                possibleWorkers.Remove(countyPopulation);
            }
            workersToRemoveFromPossibleWorkers.Clear();
        }
        public void CheckForIdle()
        {
            // Go through each person in the county.
            foreach (CountyPopulation countyPopulation in countyPopulationList)
            {
                // Go through everyone and if they are idle, helpful and loyal add them to the possibleWorkers list.
                if (countyPopulation.activity == AllEnums.Activities.Idle
                    && countyPopulation.CheckLoyalty() == true
                    && countyPopulation.CheckForPerk(AllEnums.Perks.Unhelpful) == false)
                {
                    GD.Print($"{countyName}: {countyPopulation.firstName} is idle, is loyal and is not unhelpful.");
                    possibleWorkers.Add(countyPopulation);
                }
            }
        }
        
        public void SubtractCountyResources()
        {
            // Do the math for amount used. Subtract yesterdays from todays and that is how much we have used.
            foreach (KeyValuePair<AllEnums.CountyResourceType, CountyResourceData> keyValuePair in countyResources)
            {
                amountUsedCountyResources[keyValuePair.Key].amount = countyResources[keyValuePair.Key].amount -
                    yesterdaysCountyResources[keyValuePair.Key].amount;
            }
            if (factionData.isPlayer)
            {
                GD.Print("After subtraction yesterdays vegetables is: "
                    + yesterdaysCountyResources[AllEnums.CountyResourceType.Vegetables].amount);
            }
        }

        public void OccationalNeeds()
        {
            PossiblyUseResources(herosInCountyList);
            PossiblyUseResources(armiesInCountyList);
            PossiblyUseResources(countyPopulationList);
        }

        private void PossiblyUseResources(Globals.ListWithNotify<CountyPopulation> peopleUsingResourcesList)
        {
            foreach (CountyPopulation countyPopulation in peopleUsingResourcesList)
            {
                // Check to see if they want the resource, then check if there is enough.
                foreach (KeyValuePair<AllEnums.CountyResourceType, int> keyValuePair in countyPopulation.needs)
                {
                    SkillData skillData = new();
                    // Check to see if they want the resource.

                    if (skillData.Check(keyValuePair.Value) == true)
                    {
                        //GD.Print($"Needs Checks: Passed.");
                        if (CheckEnoughOfResource(keyValuePair.Key) == true)
                        {
                            //GD.Print("There are enough resources for the needs of a person.");
                            // Use resource.
                            RemoveResourceFromCounty(keyValuePair.Key, Globals.Instance.occationalResourceUsageAmount);

                            // Add happiness.
                            countyPopulation.AddRandomHappiness(1);

                            // Set need back to zero.
                            countyPopulation.needs[keyValuePair.Key] = 0;
                        }
                        else
                        {
                            // Reduce this populations happiness.
                            countyPopulation.RemoveRandomHappiness(1);
                        }
                    }
                    else
                    {
                        // Gain 1d4 amount to the need for the next day.
                        Random random = new();
                        int needIncrease = random.Next(1, Globals.Instance.occationalNeedIncreaseAmount);
                        countyPopulation.needs[keyValuePair.Key] += needIncrease;
                        GD.Print($"Needs Checks: Failed: " + countyPopulation.needs[keyValuePair.Key]);
                    }
                }
            }
        }
        public void RemoveResourceFromCounty(AllEnums.CountyResourceType countyResourceType, int amount)
        {
            countyResources[countyResourceType].amount -= amount;

            // Update the top bar if the player has a county selected.
            if (Globals.Instance.SelectedLeftClickCounty == countyNode)
            {
                TopBarControl.Instance.UpdateResourceLabels();
            }
        }

        // This counts and compares to a global variable if there is enough of that resource.
        // We currently just have one number for a minimum of a resource, but we probably
        // should figure out a way for each different type.
        // We could actually put it in the resourceData, so each resource would know the minimum amount
        // the county needs.
        public bool CheckEnoughCountyFactionResource(AllEnums.FactionResourceType resourceType)
        {
            int amountOfResource = CountFactionResourceOfType(resourceType);
            return amountOfResource >= Globals.Instance.minimumFood;
        }

        private bool CheckEnoughOfResource(AllEnums.CountyResourceType resourceType)
        {
            bool enoughResource;
            if (countyResources[resourceType].amount >= Globals.Instance.occationalResourceUsageAmount)
            {
                enoughResource = true;
            }
            else
            {
                enoughResource = false;
            }
            return enoughResource;
        }

        private FoodLists
            GetListsOfFood()
        {
            List<CountyResourceData> perishableFoodList = [];
            List<CountyResourceData> nonperishableFoodList = [];
            foreach (CountyResourceData resourceData in countyResources.Values)
            {
                // Is food, and there is some food.
                if (resourceData.factionResourceType == AllEnums.FactionResourceType.Food
                    && resourceData.perishable == true && resourceData.amount > 0)
                {
                    //GD.Print($"Adding to list: {resourceData.name}");
                    perishableFoodList.Add(resourceData);
                }
                else if (resourceData.factionResourceType == AllEnums.FactionResourceType.Food
                    && resourceData.perishable == false && resourceData.amount > 0)
                {
                    //GD.Print($"Adding to list: {resourceData.name}");
                    nonperishableFoodList.Add(resourceData);
                }
            }
            // This a temporary list.
            FoodLists foodlists = new()
            {
                perishableFoodList = perishableFoodList,
                nonperishableFoodList = nonperishableFoodList
            };
            return foodlists;
        }

        public class FoodLists
        {
            public List<CountyResourceData> perishableFoodList = [];
            public List<CountyResourceData> nonperishableFoodList = [];
        }
        public void PopulationEatsFood(Globals.ListWithNotify<CountyPopulation> countyPopulationList, int amount)
        {
            Random random = new();
            FoodLists foodLists = GetListsOfFood();
            //GD.Print("Population List count: " + countyPopulationList.Count());
            if (countyPopulationList.Count() < 1)
            {
                //GD.PrintRich($"[pulse freq=5.0 color=green]Population Eats Food: A county population list is empty.[/pulse]");
                return;
            }
            else
            {
                List<CountyPopulation> peopleWhoNeedToDie = [];
                foreach (CountyPopulation countyPopulation in countyPopulationList)
                {
                    if (foodLists.perishableFoodList.Count > 0)
                    {
                        int randomNumber = random.Next(0, foodLists.perishableFoodList.Count);
                        // If the amount of food left is greater then zero they eat something.
                        if (foodLists.perishableFoodList[randomNumber].amount > 2)
                        {
                            foodLists.perishableFoodList[randomNumber].amount -= amount;
                            /*
                            GD.Print($"{countyPopulation.firstName} {countyPopulation.lastName} ate {amount}" +
                                $" now that county has {foodLists.perishableFoodList[randomNumber].name}" +
                                $" {foodLists.perishableFoodList[randomNumber].amount}");
                            */
                        }
                        // Person eats first, then the food is removed from the list.
                        else if (foodLists.perishableFoodList[randomNumber].amount == 1)
                        {
                            foodLists.perishableFoodList[randomNumber].amount -= amount;
                            /*
                            GD.Print($"{countyPopulation.firstName} {countyPopulation.lastName} ate {amount}" +
                                $" now that county has {foodLists.perishableFoodList[randomNumber].name}" +
                                $" {foodLists.perishableFoodList[randomNumber].amount}");
                            */
                            foodLists.perishableFoodList.Remove(foodLists.perishableFoodList[randomNumber]);
                        }
                        else
                        {
                            GD.Print($"Something is seriously fucked up.");
                        }
                    }
                    else if (foodLists.nonperishableFoodList.Count > 0)
                    {
                        int randomNumber = random.Next(0, foodLists.nonperishableFoodList.Count);
                        // If the amount of food left is greater then zero they eat something.
                        if (foodLists.nonperishableFoodList[randomNumber].amount > 2)
                        {
                            foodLists.nonperishableFoodList[randomNumber].amount -= amount;
                            /*
                            GD.Print($"{countyPopulation.firstName} {countyPopulation.lastName} ate {amount}" +
                                $" now that county has {foodLists.nonperishableFoodList[randomNumber].name}" +
                                $" {foodLists.nonperishableFoodList[randomNumber].amount}");
                            */
                        }
                        // Person eats first, then the food is removed from the list.
                        else if (foodLists.nonperishableFoodList[randomNumber].amount == 1)
                        {
                            foodLists.nonperishableFoodList[randomNumber].amount -= amount;
                            /*
                            GD.Print($"{countyPopulation.firstName} {countyPopulation.lastName} ate {amount}" +
                                $" now that county has {foodLists.nonperishableFoodList[randomNumber].name}" +
                                $" {foodLists.nonperishableFoodList[randomNumber].amount}");
                            */
                            foodLists.nonperishableFoodList.Remove(foodLists.nonperishableFoodList[randomNumber]);
                        }
                        else
                        {
                            GD.Print($"Something is seriously fucked up.");
                        }
                    }
                    else
                    {
                        // Starving!
                        //GD.PrintRich($"[rainbow]There is no food at all!");
                        GD.Print($"{countyPopulation.firstName} has starved for {countyPopulation.daysStarving} days.");
                        // This will give each population an additonal -1 to their happiness which works for now.
                        AdjustPopulationHappiness(amount, countyPopulation);
                        if (countyPopulation.daysStarving >= Globals.Instance.daysUntilDamageFromStarvation)
                        {
                            countyPopulation.hitpoints--;
                            // This should be its own method in countyPopulation that kills the population.
                            if (countyPopulation.hitpoints < 1)
                            {
                                peopleWhoNeedToDie.Add(countyPopulation);
                            }
                        }
                        countyPopulation.daysStarving++;
                    }
                    AdjustPopulationHappiness(amount, countyPopulation);
                }
                KillPeopleWhoNeedToDie(peopleWhoNeedToDie);
            }
        }

        private void KillPeopleWhoNeedToDie(List<CountyPopulation> peopleWhoNeedToDie)
        {
            foreach (CountyPopulation countyPopulation in peopleWhoNeedToDie)
            {
                countyPopulationList.Remove(countyPopulation);
                herosInCountyList.Remove(countyPopulation);
                armiesInCountyList.Remove(countyPopulation);
                deadPeopleList.Add(countyPopulation);
                GD.PrintRich($"[color=red]{countyPopulation.firstName} {countyPopulation.lastName} has croaked.[/color]");
            }
        }

        // This is sort of duplicate code.  It is almost the same as the countyPopulation.AddRandomHappiness.
        private static void AdjustPopulationHappiness(int amount, CountyPopulation countyPopulation)
        {
            //GD.Print($"{countyPopulation.firstName} happiness: {countyPopulation.Happiness}");
            if (amount == Globals.Instance.foodToGainHappiness)
            {
                countyPopulation.Happiness++;
            }
            else if (amount == Globals.Instance.foodToLoseHappiness)
            {
                countyPopulation.Happiness--;
            }
            //GD.Print($"{countyPopulation.firstName} happiness: {countyPopulation.Happiness}");
        }

        public void MoveCountyImprovementToCompletedList(List<CountyImprovementData> countyImprovementDataToRemove)
        {
            foreach (CountyImprovementData countyImprovementData in countyImprovementDataToRemove)
            {
                completedCountyImprovements.Add(countyImprovementData);
                underConstructionCountyImprovements.Remove(countyImprovementData);
            }
        }

        public void CopyCountyResourcesToYesterday()
        {
            // Creating a deep copy of the dictionary
            yesterdaysCountyResources = [];
            foreach (KeyValuePair<AllEnums.CountyResourceType, CountyResourceData> keyValuePair in countyResources)
            {
                yesterdaysCountyResources.Add(keyValuePair.Key, new CountyResourceData
                {
                    name = keyValuePair.Value.name,
                    description = keyValuePair.Value.description,
                    countyResourceType = keyValuePair.Value.countyResourceType,
                    factionResourceType = keyValuePair.Value.factionResourceType,
                    perishable = keyValuePair.Value.perishable,
                    amount = keyValuePair.Value.amount,
                    MaxAmount = keyValuePair.Value.MaxAmount,
                });
            }
            if (factionData.isPlayer)
            {
                GD.Print("Yesterday's Vegetables: " + yesterdaysCountyResources[AllEnums.CountyResourceType.Vegetables].amount);
                GD.Print("This Vegetables should be the same as yesterdays: " + countyResources[AllEnums.CountyResourceType.Vegetables].amount);
            }
        }
    }
}

