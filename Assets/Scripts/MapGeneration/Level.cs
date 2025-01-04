using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public static class Level
{
    public static float Height = 500;
    public static float Width = 500;

    public static float Scale = 1f;
    public static float IconScale = .1f;
    public static float padding = .01f;

    public static float RoomGenerationChance = .5f;

    public static int RoomLimit = 4;

    public static Sprite TreasureRoomIcon;
    public static Sprite BossRoomIcon;
    public static Sprite ShopRoomIcon;
    public static Sprite UnexploredRoom;
    public static Sprite DefaultRoomIcon;
    public static Sprite CurrentRoomIcon;
    public static Sprite SecretRoom;

    public static GameObject SecretRoomExplosion;
    public static GameObject SecretRoomDoor;
    public static bool SecretRoomExploded = false;
    public static GameObject XMark;

    public static List<Room> Rooms = new List<Room>();

    public static float RoomChangeTime = 1f;

    public static int EnemyCount = 0;

}

public class Room
{
    public int RoomNumber = 6;
    public Vector2 Location;
    public Image RoomImage;
    public Sprite RoomSprite;
    public bool Revealed;
    public bool Explored;
    public bool Cleared = true;
    
}
