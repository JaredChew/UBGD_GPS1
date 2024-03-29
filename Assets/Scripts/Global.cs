﻿public class Global {

    public static GameManager gameManager;
    public static AudioManager audiomanager;

    public static string saveFileName = "/data.johore";

    public static string audioSFX_Test = "test";

    public static string tagBox = "Box";
    public static string tagItem = "Item";
    public static string tagEnemy = "Enemy";
    public static string tagSwitch = "Switch";
    public static string tagPlayer = "Player"; 
    public static string tagGround = "Ground";
    public static string tagCharger = "Charger";
    public static string tagAnimation = "Animation";
    public static string tagButton = "Button";

    public static string controlsMap = "Map";
    public static string controlsJump = "Jump";
    public static string controlsHide = "Hide"; 
    public static string controlsClimb = "Climb";
    public static string controlsThrow = "Throw";
    public static string controlsPause = "Pause";
    public static string controlsRecall = "Recall";
    public static string controlsCrouch = "Crouch";
    public static string controlsLevitate = "Levitate";
    public static string controlsInteract = "Interact";
    public static string controlsLeftRight = "Horizontal";

    public static string standardEnemyAnim_isDisabled = "isDisabled";

    public enum Scenes { splashscreen = 0, mainMenu = 1, prototype = 3, demo = 4, game = 2 };
    public enum Areas { area1, area2, area3, area4, area5, area6, area7, area8, area9 };
    public enum BoxAbilities { hidePlayer, electricCharge, levitate };
    public enum CheckpointLocation { none, checkPoint1, checkPoint2, checkPoint3, checkpoint4, checkpoint5 };

}
