using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Skill Data",menuName = "Scriptable/Skill Data",order = int.MaxValue)]

public class SkillData : ScriptableObject
{
    [SerializeField]
    private string skillName;
    public string SkillName { get { return skillName; } }

    [SerializeField]
    private Sprite skillImage;
    public Sprite SkillImage { get {  return skillImage; } }

    [SerializeField] int damage;
    public int Damage { get { return damage; } }

    [SerializeField] float range;
    public float Range { get { return range; } }
}
