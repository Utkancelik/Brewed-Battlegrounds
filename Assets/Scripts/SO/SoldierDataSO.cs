using UnityEngine;

[CreateAssetMenu(fileName = "SoldierDataSO", menuName = "SoldierDataSO")]
public class SoldierDataSO : ScriptableObject
{
    [Header("Basic Info")]
    [SerializeField] private string soldierName;
    [SerializeField] private Sprite soldierIcon;
    [SerializeField] private GameObject prefab;
    [SerializeField] private bool isUnlocked;
    [SerializeField] private int unlockCost;
    [SerializeField] private int era;
    [SerializeField] private int foodCost;

    [Header("Stats")]
    [SerializeField] private int health;
    [SerializeField] private float speed;
    [SerializeField] private float attackRange;
    [SerializeField] private float detectRange;
    [SerializeField] private int damage;

    public string SoldierName => soldierName;
    public Sprite SoldierIcon => soldierIcon;
    public GameObject Prefab => prefab;
    public bool IsUnlocked { get => isUnlocked; set => isUnlocked = value; }
    public int UnlockCost => unlockCost;
    public int Era => era;
    public int FoodCost => foodCost;
    public int Health => health;
    public float Speed => speed;
    public float AttackRange => attackRange;
    public float DetectRange => detectRange;
    public int Damage => damage;
}

