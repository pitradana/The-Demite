﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheDemiteServer
{
    class Player
    {
        private string playerName;
        private float latitude;
        private float longitude;
        private MapControllerServer mapControl;
        private Pet pet;
        private bool isActive;

        public Player(string playerName, float playerLatitude, float playerLongitude, string petName, float petPosX, float petPosY)
        {
            this.playerName = playerName;
            this.latitude = playerLatitude;
            this.longitude = playerLongitude;
            this.mapControl = new MapControllerServer();
            this.pet = new Pet(petName, petPosX, petPosY);
            this.isActive = true;
        }

        public bool CheckAndCreateMap()
        {
            bool needToCreate = this.mapControl.ConvertLocationAndCheck(this.latitude, this.longitude);
            if (needToCreate)
            {
                Console.WriteLine("creating map");
                this.mapControl.CreateMap();
            }
            else
            {
                this.mapControl.SetMapReady(true);
            }

            return needToCreate;
        }

        public void SetPlayerName(string playerName)
        {
            this.playerName = playerName;
        }

        public string GetPlayerName()
        {
            return this.playerName;
        }

        public void SetLatitude(float latitude)
        {
            this.latitude = latitude;
        }

        public float GetLatitude()
        {
            return this.latitude;
        }

        public void SetLongitude(float longitude)
        {
            this.longitude = longitude;
        }

        public float GetLongitude()
        {
            return this.longitude;
        }

        public void SetMapController(MapControllerServer mapControl)
        {
            this.mapControl = mapControl;
        }

        public MapControllerServer GetMapController()
        {
            return this.mapControl;
        }

        public void SetPet(string petName, float posX, float posY)
        {
            this.pet = new Pet(petName, posX, posY);
        }

        public Pet GetPet()
        {
            return this.pet;
        }

        public void SetIsActive(bool isActive)
        {
            this.isActive = isActive;
        }

        public bool GetIsActive()
        {
            return this.isActive;
        }
    }
}
