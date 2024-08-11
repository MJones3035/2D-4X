using Godot;
using System.Collections.Generic;

namespace PlayerSpace
{
    [GlobalClass]

    public partial class AttributeData : Resource
    {
        [Export] public AllEnums.Attributes attribute;
        [Export] public string attributeAbbreviation;
        [Export] public string attributeName;
        [Export] public string attributeDescription;
        [Export] public int attributeLevel;

        /// <summary>
        /// Return an int that is an attribute bonus.  It can return a negative, 10, or 1.
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="ones"></param>
        /// <param name="negative"></param>
        /// <returns></returns>
        public static int ApplyAttributeBonuses(int attribute, bool ones, bool negative)
        {
            // List of all of the ranges for attribute bonuses.
            List<(int min, int max, int bonus)> attributeBonuses =
            [
                (1, 10, -20),
                (11, 20, -15),
                (21, 30, -10),
                (31, 40, -5),
                (41, 60, 0),
                (61, 70, 5),
                (71, 80, 10),
                (81, 90, 15),
                (91, 100, 20)
            ];
            int bonus = 0;
            // If the number is between the min and the max it gets the bonus.
            foreach ((int min, int max, int bonusValue) in attributeBonuses)
            {
                if (attribute >= min && attribute <= max)
                {
                    bonus = bonusValue;
                    break;
                }
            }
            if(ones == true)
            {
                bonus /= 5;
            }
            if (negative == true)
            {
                bonus *= -1;
            }
            //GD.PrintRich($"[rainbow]Attribute Bonus: {bonus}");
            return bonus;
        }

        public static Godot.Collections.Dictionary<AllEnums.Attributes, AttributeData> NewCopy()
        {
            Godot.Collections.Dictionary<AllEnums.Attributes, AttributeData> newAttributes = [];
            foreach (AttributeData attributeData in AllAttributes.Instance.allAttributes)
            {
                newAttributes.Add(attributeData.attribute, new AttributeData
                {
                    attribute = attributeData.attribute,
                    attributeName = attributeData.attributeName,
                    attributeAbbreviation = attributeData.attributeAbbreviation,
                    attributeDescription = attributeData.attributeDescription,
                    attributeLevel = attributeData.attributeLevel,
                });
            }
            return newAttributes;
        }
    }
}


