using Godot;
using System;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class SkillData : Resource
    {
        [Export] public string skillName;
        [Export] public int skillLevel;
        [Export] public int amountLearned;
        [Export] public bool isCombatSkill;
        [Export] public AllEnums.SkillType skillType;
    }
}