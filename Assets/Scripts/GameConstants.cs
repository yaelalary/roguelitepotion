using UnityEngine;

/// <summary>
/// Contains all game constants and configuration values
/// Centralized place for game balance and fixed values
/// </summary>
public static class GameConstants
{
    // POTION SYSTEM
    public const int MAX_INGREDIENTS_PER_POTION = 4;
    public const int MIN_INGREDIENTS_PER_POTION = 1;
    public const int MAX_SHELVES = 3;
    
    // POTION LEVELS
    public const int MIN_POTION_LEVEL = 1;
    public const int MAX_POTION_LEVEL = 5;
    public const int MIN_POTION_SUB_LEVEL = 1;
    public const int MAX_POTION_SUB_LEVEL = 3;
    
    // POTION DURATIONS (in seconds)
    public const int MIN_POTION_DURATION = 1;
    public const int MAX_POTION_DURATION = 10;
    public const int DEFAULT_POTION_DURATION = 1;
    
    // FAMILY INITIALS
    public const string MAGIC_FAMILY = "Magic";
    public const string NATURAL_FAMILY = "Natural";

    // SUBFAMILY INITIALS
    public const string PLANT_SUBFAMILY = "Plant";
    public const string ANIMAL_SUBFAMILY = "Animal";
    public const string MINERAL_SUBFAMILY = "Mineral";
}
