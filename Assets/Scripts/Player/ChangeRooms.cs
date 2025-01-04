using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Android;

public class ChangeRooms : MonoBehaviour
{
    public Transform Rooms;
    public float RoomSpawnOffset = 20;

    private Sprite previousImage;

    private void Start()
    {
        previousImage = Level.DefaultRoomIcon;
        EnableDoors(Player.CurrentRoom);
    }

    void ChangeRoomIcon(Room CurrentRoom, Room NewRoom)
    {
        CurrentRoom.RoomImage.sprite = previousImage;
        previousImage = NewRoom.RoomImage.sprite;
        NewRoom.RoomImage.sprite = Level.CurrentRoomIcon;
    }

    bool changeroomcooldown = false;

    void EndChangeRoomCooldown()
    {
        changeroomcooldown = false;
    }


    public static void RedrawRevealedRooms()
    {
        foreach (Room room in Level.Rooms)
        {
            if (!room.Revealed && !room.Explored) room.RoomImage.color = new Color(1, 1, 1, 0);
            if (room.Revealed && !room.Explored && room.RoomNumber > 5) room.RoomImage.sprite = Level.UnexploredRoom;
            if (room.Explored && room.RoomNumber > 5) room.RoomImage.sprite = Level.DefaultRoomIcon;
            if (room.Explored || room.Revealed) room.RoomImage.color = new Color(1, 1, 1, 1);
            Player.CurrentRoom.RoomImage.sprite = Level.CurrentRoomIcon;
        }

    }

    public static void RevealRooms(Room R)
    {
        foreach (Room room in Level.Rooms)
        {
            //left
            if (room.Location == R.Location + new Vector2(-1, 0) && room.RoomNumber != 4)
            {
                room.Revealed = true;
            }

            //right
            if (room.Location == R.Location + new Vector2(1, 0) && room.RoomNumber != 4)
            {
                room.Revealed = true;
            }

            //down
            if (room.Location == R.Location + new Vector2(0, -1) && room.RoomNumber != 4)
            {
                room.Revealed = true;
            }
            //up
            if (room.Location == R.Location + new Vector2(0, 1) && room.RoomNumber != 4)
            {
                room.Revealed = true;
            }
        }
    }


    void CheckDoor(Vector2 NewLocation, string Direction, Vector3 RoomOffset)
    {

        //where are we?
        Vector2 Location = Player.CurrentRoom.Location;

        //where are we going?
        Location = Location + NewLocation;

        if (Level.Rooms.Exists(x => x.Location == Location))
        {
            Room R = Level.Rooms.First(x => x.Location == Location);

            //disable the room that you're in
            Rooms.Find(Player.CurrentRoom.RoomNumber.ToString()).gameObject.SetActive(false);



            //Find the new room and activate it
            GameObject NewRoom = Rooms.Find(R.RoomNumber.ToString()).gameObject;
            NewRoom.SetActive(true);

            //Move the player to the door area where he would be coming through
            Player.Controller.enabled = false;
            transform.position = NewRoom.transform.Find("Doors").transform.Find(Direction).position + RoomOffset;
            Player.Controller.enabled = true;

            ChangeRoomIcon(Player.CurrentRoom, R);

            Player.CurrentRoom = R;

            EnableDoors(R);

            Player.CurrentRoom.Explored = true;

            RevealRooms(R);
            RedrawRevealedRooms();


            Transform Enemies = NewRoom.transform.Find("Enemies");
            if (Enemies != null)
            {
                Player.CurrentRoom.Cleared = false;
                Level.EnemyCount = Enemies.childCount;
            }
            else
            {
                Level.EnemyCount = 0;

                Transform Doors = NewRoom.transform.Find("Doors");


                Animator A;
                Doors.Find("LeftDoor").TryGetComponent<Animator>(out A);
                A.enabled = true;
                Doors.Find("RightDoor").TryGetComponent<Animator>(out A);
                A.enabled = true;
                Doors.Find("TopDoor").TryGetComponent<Animator>(out A);
                A.enabled = true;
                Doors.Find("BottomDoor").TryGetComponent<Animator>(out A);
                A.enabled = true;
            }





        }
    }


    bool CheckIfRoomAlreadyHasX(Transform T)
    {
        for (int i = 0; i < T.childCount; ++i)
        {
            if (T.GetChild(i).name.Contains("DrawX"))
            {
                return true;
            }
        }

        return false;
    }

    void EnableDoors(Room R)
    {
        Transform T = Rooms.Find(R.RoomNumber.ToString());
        Transform Doors = T.Find("Doors");

        for (int i = 0; i < Doors.childCount; i++)
        {
            Doors.GetChild(i).gameObject.SetActive(false);
        }

        //Check what doors should be there

        //Left
        {
            Vector2 NewPosition = R.Location + new Vector2(-1, 0);
            if (Level.Rooms.Exists(x => x.Location == NewPosition))
            {
                if (Level.Rooms.First(x => x.Location == NewPosition).RoomNumber == 4)
                {

                    if (Level.SecretRoomExploded)
                    {
                        GameObject GO = Doors.Find("LeftDoor").gameObject;
                        GameObject SecretDoor = Instantiate(Level.SecretRoomDoor, GO.transform.position, GO.transform.rotation * Quaternion.Euler(0, 90, 0), Doors);
                        SecretDoor.name = "LeftDoor";
                    }
                    else
                    {
                        if (!CheckIfRoomAlreadyHasX(T))
                        {

                            GameObject GO = Doors.Find("LeftDoor").gameObject;
                            Instantiate(Level.XMark, GO.transform.position, GO.transform.rotation * Quaternion.Euler(0, 90, 0), T);
                        }

                    }
                }
                else
                {
                    Doors.Find("LeftDoor").gameObject.SetActive(true);

                }
            }
        }

        //Up
        {
            Vector2 NewPosition = R.Location + new Vector2(0, 1);
            if (Level.Rooms.Exists(x => x.Location == NewPosition))
            {
                if (Level.Rooms.First(x => x.Location == NewPosition).RoomNumber == 4)
                {
                    if (Level.SecretRoomExploded)
                    {
                        GameObject GO = Doors.Find("TopDoor").gameObject;
                        GameObject SecretDoor = Instantiate(Level.SecretRoomDoor, GO.transform.position, GO.transform.rotation * Quaternion.Euler(0, 90, 0), Doors);
                        SecretDoor.name = "TopDoor";
                    }
                    else
                    {
                        if (!CheckIfRoomAlreadyHasX(T))
                        {
                            GameObject GO = Doors.Find("TopDoor").gameObject;
                            Instantiate(Level.XMark, GO.transform.position, GO.transform.rotation * Quaternion.Euler(0, 90, 0), T);
                        }
                    }

                }
                else
                {
                    Doors.Find("TopDoor").gameObject.SetActive(true);
                }
            }
        }

        //Down
        {
            Vector2 NewPosition = R.Location + new Vector2(0, -1);
            if (Level.Rooms.Exists(x => x.Location == NewPosition))
            {
                if (Level.Rooms.First(x => x.Location == NewPosition).RoomNumber == 4)
                {
                    if (Level.SecretRoomExploded)
                    {
                        GameObject GO = Doors.Find("BottomDoor").gameObject;
                        GameObject SecretDoor = Instantiate(Level.SecretRoomDoor, GO.transform.position, GO.transform.rotation * Quaternion.Euler(0, 90, 0), Doors);
                        SecretDoor.name = "BottomDoor";
                    }
                    else
                    {
                        if (!CheckIfRoomAlreadyHasX(T))
                        {
                            GameObject GO = Doors.Find("BottomDoor").gameObject;
                            Instantiate(Level.XMark, GO.transform.position, GO.transform.rotation * Quaternion.Euler(0, 90, 0), T);
                        }
                    }

                }
                else
                {
                    Doors.Find("BottomDoor").gameObject.SetActive(true);
                }
            }
        }

        //Right
        {
            Vector2 NewPosition = R.Location + new Vector2(1, 0);
            if (Level.Rooms.Exists(x => x.Location == NewPosition))
            {
                if (Level.Rooms.First(x => x.Location == NewPosition).RoomNumber == 4)
                {
                    if (Level.SecretRoomExploded)
                    {
                        GameObject GO = Doors.Find("RightDoor").gameObject;
                        GameObject SecretDoor = Instantiate(Level.SecretRoomDoor, GO.transform.position, GO.transform.rotation * Quaternion.Euler(0, 90, 0), Doors);
                        SecretDoor.name = "RightDoor";

                    }
                    else
                    {
                        if (!CheckIfRoomAlreadyHasX(T))
                        {
                            GameObject GO = Doors.Find("RightDoor").gameObject;
                            Instantiate(Level.XMark, GO.transform.position, GO.transform.rotation * Quaternion.Euler(0, 90, 0), T);
                        }
                    }
                }
                else
                {
                    Doors.Find("RightDoor").gameObject.SetActive(true);
                }
            }
        }


    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (changeroomcooldown ||
            hit.gameObject.layer == LayerMask.NameToLayer("Floor") ||
            Player.CurrentRoom.Cleared != true
            )
        {
            return;
        }
        else
        {
            changeroomcooldown = true;
            Invoke(nameof(EndChangeRoomCooldown), Level.RoomChangeTime);
        }


        if (hit.gameObject.name == "LeftDoor")
        {
            CheckDoor(new Vector2(-1, 0), "RightDoor", new Vector3(-RoomSpawnOffset, 0, 0));
        }

        else if (hit.gameObject.name == "RightDoor")
        {
            CheckDoor(new Vector2(1, 0), "LeftDoor", new Vector3(RoomSpawnOffset, 0, 0));
        }

        else if (hit.gameObject.name == "TopDoor")
        {
            CheckDoor(new Vector2(0, 1), "BottomDoor", new Vector3(0, 0, RoomSpawnOffset));
        }

        else if (hit.gameObject.name == "BottomDoor")
        {
            CheckDoor(new Vector2(0, -1), "TopDoor", new Vector3(0, 0, -RoomSpawnOffset));
        }




    } }
