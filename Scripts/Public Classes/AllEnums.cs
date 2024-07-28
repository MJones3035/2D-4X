namespace PlayerSpace
{
    public class AllEnums
    {
        public enum Activities
        {
            Build,
            Combat,
            Idle,
            Move,
            Research,
            Scavenge,
            Work,
        }
        public enum Attributes
        {
            Agility,
            Awareness,
            Charisma,
            Endurance,
            Intelligence,
            Looks,
            MentalStrength,
            PhysicalStrength,
        }

        public enum CountyImprovementStatus
        {
            None,
            Complete,
            UnderConstruction,
        }
        public enum CountyResourceType
        {
            None,
            CannedFood,
            Fish,
            Remnants,
            Vegetables,
            Wood,
        }

        public enum LearningSpeed
        {
            slow,
            medium,
            fast,
        }

        // Scrap and wood should be combined into building materials.
        public enum FactionResourceType
        {
            None,
            BuildingMaterial,
            Food,
            Influence,
            Money,
            Remnants
        }

        public enum Perks
        {
            LeaderOfPeople,
            Unhelpful,
        }

        public enum Province
        {
            Oregon,
            Washington,
        }

        public enum ResearchTiers
        {
            One,
            Two,
            Three,
        }

        public enum Skills
        {
            Construction,
            Cool,
            Farm,
            Fish,
            Labor, // Possibly make two skills, one of them hauling.
            Lumberjack,
            Research,
            Rifle,
            Scavenge,
        }

        public enum Terrain
        {
            Coast,
            Desert,
            Forest,
            Mountain,
            Plain,
            River,
            Ruin,
        }
    }
}