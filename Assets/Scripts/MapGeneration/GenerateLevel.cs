using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GenerateLevel : MonoBehaviour
{
    public Sprite CurrentRoom;
    public Sprite BRoom;
    public Sprite EmptyRoom;
    public Sprite ShopRoom;
    public Sprite TreasureRoom;
    public Sprite UnexploredRoom;
    public Sprite SecretRoom;

    private bool BossRoomGenerated = false;


    void DrawRoomOnMap(Room R)
    {
        string TileName = "MapTile";
        if (R.RoomNumber == 1) TileName = "BossRoomTile";
        if (R.RoomNumber == 2) TileName = "ShopRoomTile";
        if (R.RoomNumber == 3) TileName = "ItemRoomTile";
        GameObject MapTile = new GameObject(TileName);
        Image RoomImage = MapTile.AddComponent<Image>();
        RoomImage.sprite = R.RoomSprite;
        R.RoomImage = RoomImage;
        RectTransform rectTransform = RoomImage.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(Level.Height, Level.Width) * Level.IconScale;
        rectTransform.position = R.Location * (Level.IconScale * Level.Height * Level.Scale + (Level.padding * Level.Height * Level.Scale));
        RoomImage.transform.SetParent(transform, false);

        Level.Rooms.Add(R);
        Debug.Log("Drawing Room:" + R.RoomNumber + " at location:" + R.Location);
    }


    int RandomRoomNumber()
    {
        return Random.Range(6,GameObject.Find("Rooms").transform.childCount);
    }


    bool CheckIfRoomExists(Vector2 v)
    {
        return (Level.Rooms.Exists(x => x.Location == v));
    }


    bool CheckIfRoomsAroundGeneratedRoom(Vector2 v, string direction)
    {

        switch (direction)
        {
            case "Right":
                {
                    //Check Down,left,and up
                    if (Level.Rooms.Exists(x => x.Location == new Vector2(v.x - 1, v.y)) ||
                       Level.Rooms.Exists(x => x.Location == new Vector2(v.x, v.y - 1)) ||
                       Level.Rooms.Exists(x => x.Location == new Vector2(v.x, v.y + 1)))
                        return true;
                    break;
                }
            case "Left":
                {
                    //Check Down,Right,and up
                    if (Level.Rooms.Exists(x => x.Location == new Vector2(v.x + 1, v.y)) ||
                       Level.Rooms.Exists(x => x.Location == new Vector2(v.x, v.y - 1)) ||
                       Level.Rooms.Exists(x => x.Location == new Vector2(v.x, v.y + 1)))
                        return true;
                    break;
                }
            case "Up":
                {
                    //Check Down,Right,and Left
                    if (Level.Rooms.Exists(x => x.Location == new Vector2(v.x + 1, v.y)) ||
                       Level.Rooms.Exists(x => x.Location == new Vector2(v.x -1, v.y)) ||
                       Level.Rooms.Exists(x => x.Location == new Vector2(v.x, v.y - 1)))
                        return true;
                    break;
                }
            case "Down":
                {
   
                    if (Level.Rooms.Exists(x => x.Location == new Vector2(v.x, v.y+1)) ||
                       Level.Rooms.Exists(x => x.Location == new Vector2(v.x -1, v.y)) ||
                       Level.Rooms.Exists(x => x.Location == new Vector2(v.x + 1, v.y)))
                        return true;
                    break;
                }

        }



        return false;
    }



    int failsafe = 0;


    void Generate(Room room)
    {
        failsafe++;
        if (failsafe > 50)
        {
            return;
        }

        DrawRoomOnMap(room);


        //Left
        if (Random.value > Level.RoomGenerationChance)
        {
            Room newRoom = new Room();
            newRoom.Location = new Vector2(-1, 0) + room.Location;
            newRoom.RoomSprite = Level.DefaultRoomIcon;
            newRoom.RoomNumber = RandomRoomNumber();

            if (!CheckIfRoomExists(newRoom.Location))
            {
                if (!CheckIfRoomsAroundGeneratedRoom(newRoom.Location, "Right"))
                {
                    if (Mathf.Abs(newRoom.Location.x) < Level.RoomLimit && Mathf.Abs(newRoom.Location.y) < Level.RoomLimit)
                    {
                   
                        Generate(newRoom);
                    }

                }
            }
        }

        //Right
        if (Random.value > Level.RoomGenerationChance)
        {
            Room newRoom = new Room();
            newRoom.Location = new Vector2(1, 0) + room.Location;
            newRoom.RoomSprite = Level.DefaultRoomIcon;
            newRoom.RoomNumber = RandomRoomNumber();
            if (!CheckIfRoomExists(newRoom.Location))
            {
                if (!CheckIfRoomsAroundGeneratedRoom(newRoom.Location, "Left"))
                {
                    if (Mathf.Abs(newRoom.Location.x) < Level.RoomLimit && Mathf.Abs(newRoom.Location.y) < Level.RoomLimit)
                    {
          
                        Generate(newRoom);
                    }
                }
            }
        }

        //Up
        if (Random.value > Level.RoomGenerationChance)
        {
            Room newRoom = new Room();
            newRoom.Location = new Vector2(0, 1) + room.Location;
            newRoom.RoomSprite = Level.DefaultRoomIcon;
            newRoom.RoomNumber = RandomRoomNumber();
            if (!CheckIfRoomExists(newRoom.Location))
            {
                if (!CheckIfRoomsAroundGeneratedRoom(newRoom.Location, "Down"))
                {
                    if (Mathf.Abs(newRoom.Location.x) < Level.RoomLimit && Mathf.Abs(newRoom.Location.y) < Level.RoomLimit)
                    {
         
                        Generate(newRoom);
                    }
                }
            }
        }
        //Down
        if (Random.value > Level.RoomGenerationChance)
        {
            Room newRoom = new Room();
            newRoom.Location = new Vector2(0, -1) + room.Location;
            newRoom.RoomSprite = Level.DefaultRoomIcon;
            newRoom.RoomNumber = RandomRoomNumber();
            if (!CheckIfRoomExists(newRoom.Location))
            {
                if (!CheckIfRoomsAroundGeneratedRoom(newRoom.Location, "Up"))
                {
                    if (Mathf.Abs(newRoom.Location.x) < Level.RoomLimit && Mathf.Abs(newRoom.Location.y) < Level.RoomLimit)
                    {
          
                        Generate(newRoom);
                    }
                }
            }
        }


        
    }

    private void GenerateBossRoom()
    {
        float MaxNumber = 0;
        Vector2 FarthestRoom = Vector2.zero;

        foreach(Room R in Level.Rooms)
        {
            if(Mathf.Abs(R.Location.x) + Mathf.Abs(R.Location.y) >= MaxNumber)
            {
                MaxNumber = Mathf.Abs(R.Location.x) + Mathf.Abs(R.Location.y);
                FarthestRoom = R.Location;
            }
            

        }

        Debug.Log("Boss Farthest room:" + FarthestRoom);

        Room BossRoom = new Room();
        BossRoom.RoomSprite = Level.BossRoomIcon;
        BossRoom.RoomNumber = 1;

        //Left
        if (!CheckIfRoomExists(FarthestRoom + new Vector2(-1, 0)))
        {
            if (!CheckIfRoomsAroundGeneratedRoom(FarthestRoom + new Vector2(-1, 0), "Right"))
            {
                BossRoom.Location = FarthestRoom + new Vector2(-1, 0);
            }
        }

        //Right
        if (!CheckIfRoomExists(FarthestRoom + new Vector2(1, 0)))
        {
            if (!CheckIfRoomsAroundGeneratedRoom(FarthestRoom + new Vector2(1, 0), "Left"))
            {
                BossRoom.Location = FarthestRoom + new Vector2(1, 0);
            }
        }

        //Up
        if (!CheckIfRoomExists(FarthestRoom + new Vector2(0, 1)))
        {
            if (!CheckIfRoomsAroundGeneratedRoom(FarthestRoom + new Vector2(0, 1), "Down"))
            {
                BossRoom.Location = FarthestRoom + new Vector2(0, 1);
            }
        }
        //Down
        if (!CheckIfRoomExists(FarthestRoom + new Vector2(0, -1)))
        {
            if (!CheckIfRoomsAroundGeneratedRoom(FarthestRoom + new Vector2(0, -1), "Up"))
            {
                BossRoom.Location = FarthestRoom + new Vector2(0, -1);
            }
        }

        DrawRoomOnMap(BossRoom);

    }

    void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        System.Random rng = new System.Random();

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }

    }

    private bool GenerateSpecialRoom(Sprite MapIcon, int RoomNumber)
    {
        List<Room> ShuffledList = new List<Room>(Level.Rooms);
        ShuffleList(ShuffledList);

        Room SpecialRoom = new Room();
        SpecialRoom.RoomSprite = MapIcon;
        SpecialRoom.RoomNumber = RoomNumber;

        bool FoundAvailableLocation = false;

        foreach (Room R in ShuffledList)
        {
            Vector2 SpecialRoomLocation = R.Location;


            if (R.RoomNumber < 6) continue;


            //Left
            if (!CheckIfRoomExists(SpecialRoomLocation + new Vector2(-1, 0)))
            {
                if (!CheckIfRoomsAroundGeneratedRoom(SpecialRoomLocation + new Vector2(-1, 0), "Right"))
                {
                    SpecialRoom.Location = SpecialRoomLocation + new Vector2(-1, 0);
                    FoundAvailableLocation = true;
                }
            }

            //Right
            else if (!CheckIfRoomExists(SpecialRoomLocation + new Vector2(1, 0)))
            {
                if (!CheckIfRoomsAroundGeneratedRoom(SpecialRoomLocation + new Vector2(1, 0), "Left"))
                {
                    SpecialRoom.Location = SpecialRoomLocation + new Vector2(1, 0);
                    FoundAvailableLocation = true;
                }
            }

            //Up
            else if (!CheckIfRoomExists(SpecialRoomLocation + new Vector2(0, 1)))
            {
                if (!CheckIfRoomsAroundGeneratedRoom(SpecialRoomLocation + new Vector2(0, 1), "Down"))
                {
                    SpecialRoom.Location = SpecialRoomLocation + new Vector2(0, 1);
                    FoundAvailableLocation = true;
                }
            }
            //Down
            else if (!CheckIfRoomExists(SpecialRoomLocation + new Vector2(0, -1)))
            {
                if (!CheckIfRoomsAroundGeneratedRoom(SpecialRoomLocation + new Vector2(0, -1), "Up"))
                {
                    SpecialRoom.Location = SpecialRoomLocation + new Vector2(0, -1);
                    FoundAvailableLocation = true;
                }
            }

            if (FoundAvailableLocation) { 
                DrawRoomOnMap(SpecialRoom);
                return true;
            }

        }

        return false;

    }

    private bool GenerateSecretRoom()
    {
        List<Room> ShuffledList = new List<Room>(Level.Rooms);
        ShuffleList(ShuffledList);

        foreach (Room R in ShuffledList)
        {
            // x and y < 3 and > -3 starting room is at 0,0
            if (Mathf.Abs(R.Location.x) > 2 || Mathf.Abs(R.Location.y) > 2 || R.Location == Vector2.zero)
            {
                continue;
            }

            // Define the directions
            Vector2[] directions = { new Vector2(-1, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(0, -1) };

            foreach (Vector2 direction in directions)
            {
                Vector2 newLocation = R.Location + direction;

                // Check if a room already exists at the new location
                if (!Level.Rooms.Exists(x => x.Location == newLocation))
                {
                    if (Mathf.Abs(newLocation.x) > 1 || Mathf.Abs(newLocation.y) >1) //Prevents it from being drawn next to the start room.
                    {
                        CreateNewRoom(newLocation);
                        return true;
                    }
                }
            }
        }


        return false;
    }


    //Used for Secret Room
    void CreateNewRoom(Vector2 location)
    {
        Room SR = new Room
        {
            Location = location,
            RoomSprite = Level.SecretRoom,
            Explored = false,
            Revealed = false,
            RoomNumber = 4
        };

        DrawRoomOnMap(SR);
    }

    private void Awake()
    {
        Level.DefaultRoomIcon = EmptyRoom;
        Level.BossRoomIcon = BRoom;
        Level.CurrentRoomIcon = CurrentRoom;
        Level.ShopRoomIcon = ShopRoom;
        Level.TreasureRoomIcon = TreasureRoom;
        Level.UnexploredRoom = UnexploredRoom;
        Level.SecretRoom = SecretRoom;
    }

    int maxtries = 0;

    void Start()
    {

        maxtries++;

        Room StartRoom = new Room();
        StartRoom.Location = new Vector2(0, 0);
        StartRoom.RoomSprite = Level.CurrentRoomIcon;
        StartRoom.Explored = true;
        StartRoom.Revealed = true;
        
        StartRoom.RoomNumber = 0;

        Player.CurrentRoom = StartRoom;

        //Drawing the starting room
        DrawRoomOnMap(StartRoom);

        //Left
        if (Random.value > Level.RoomGenerationChance)
        {
            Room newRoom = new Room();
            newRoom.Location = new Vector2(-1, 0);
            newRoom.RoomSprite = Level.DefaultRoomIcon;
            newRoom.RoomNumber = RandomRoomNumber();
            if (!CheckIfRoomExists(newRoom.Location))
            {
                if (!CheckIfRoomsAroundGeneratedRoom(newRoom.Location, "Right"))
                    Generate(newRoom);
            } 
        }

        //Right
        if (Random.value > Level.RoomGenerationChance)
        {
            Room newRoom = new Room();
            newRoom.Location = new Vector2(1, 0);
            newRoom.RoomSprite = Level.DefaultRoomIcon;
            newRoom.RoomNumber = RandomRoomNumber();
            if (!CheckIfRoomExists(newRoom.Location))
            {
                if (!CheckIfRoomsAroundGeneratedRoom(newRoom.Location, "Left"))
                    Generate(newRoom);
            }
        }

        //Up
        if (Random.value > Level.RoomGenerationChance)
        {
            Room newRoom = new Room();
            newRoom.Location = new Vector2(0, 1);
            newRoom.RoomSprite = Level.DefaultRoomIcon;
            newRoom.RoomNumber = RandomRoomNumber();
            if (!CheckIfRoomExists(newRoom.Location))
                {
                    if (!CheckIfRoomsAroundGeneratedRoom(newRoom.Location, "Down"))
                        Generate(newRoom);
                }
        }
        //Down
        if (Random.value > Level.RoomGenerationChance)
        {
            Room newRoom = new Room();
            newRoom.Location = new Vector2(0, -1);
            newRoom.RoomSprite = Level.DefaultRoomIcon;
            newRoom.RoomNumber = RandomRoomNumber();
            if (!CheckIfRoomExists(newRoom.Location))
                    {
                        if (!CheckIfRoomsAroundGeneratedRoom(newRoom.Location, "Up"))
                            Generate(newRoom);
                    }
        }

        GenerateBossRoom();

       bool treasure = GenerateSpecialRoom(Level.TreasureRoomIcon, 3);
       bool shop = GenerateSpecialRoom(Level.ShopRoomIcon, 2);
        bool secret = GenerateSecretRoom();

        if (!treasure || !shop || !secret)
        {
            if (maxtries > 15) return;

            Regenerate();

        }
        else
        {
            ChangeRooms.RevealRooms(StartRoom);
            ChangeRooms.RedrawRevealedRooms();
        }

    }


    bool regenerating = false;
    void StopRegenerating()
    {
        regenerating = false;
    }

   void Regenerate()
    {
        regenerating = true;
        failsafe = 0;
        Level.Rooms.Clear();
        Invoke(nameof(StopRegenerating), 1);
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            Destroy(child.gameObject);
        }


        Start();
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.Tab) && !regenerating)
        {
            Regenerate();
        }

        if (Input.GetKey(KeyCode.P) && !regenerating)
        {
            regenerating = true;
            Invoke(nameof(StopRegenerating), 1);

            string log = "Room List:\n-----------------\n";
     
            foreach (Room R in Level.Rooms)
            {
                log += "Room#:" + R.RoomNumber + " Location: " + R.Location + "\n";
            }
            Debug.Log(log);

            Debug.Log(Player.State);
            
        }

       

    }

}
