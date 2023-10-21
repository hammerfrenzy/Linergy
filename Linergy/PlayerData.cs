using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.GamerServices;

namespace Linergy
{
    public class PlayerData
    {
        List<List<int>> medals;
        float currentEnergyAmount;
        float currentShieldAmount;
        int playerLevel;
        int unlocked;
        int levelRequirement;
        int experience;
        int totalScore;
        int baseExpReq;
        int combo;
        int currentScore;
        int currentChapter;
        int currentWorld;
        private int toNextLevel;
        private float drawTimeStart = 0;
        private bool drawLevelUp = false;

        public PlayerData()
        {
            medals = new List<List<int>>();
            for (int x = 0; x < 6; x++)
            {
                medals.Add(new List<int>());
                for (int y = 0; y < 4; y++)
                    medals[x].Add(0);
            }
            playerLevel = 1;
            currentEnergyAmount = 25;
            currentShieldAmount = 0;
            currentScore = 0;
            currentWorld = 0;
            combo = 1;
            baseExpReq = 200;
            toNextLevel = baseExpReq;
            unlocked = 1;
            Initialize(); //Read totalScore and currentChapter, and unlocked from IsolatedStorage
        }

        public void Update(GameTime gameTime)
        {
            if (toNextLevel - experience <= 0)
                LevelUp(gameTime);

            if (drawLevelUp)
                drawTimeStart += gameTime.ElapsedGameTime.Milliseconds;
            if (drawTimeStart > 1000)
            {
                drawTimeStart = 0;
                drawLevelUp = false;
            }
        }

        public void AddEnergy(float amount)
        {
            //Cap energy at 100, and don't add if in Overcharge unless we have the 10% upgrade from world 3
            if (playerLevel < Game1.LevelCap)
                currentEnergyAmount += amount;
            else if (playerLevel == Game1.LevelCap && currentWorld > 1)
                currentEnergyAmount += .01f * amount;
            if (currentEnergyAmount > 100)
                currentEnergyAmount = 100;

            //Add shield if we're in world 5 or later
            if (currentWorld >= 4)
            {
                currentShieldAmount += .5f * amount;
                if (currentShieldAmount > 50)
                    currentShieldAmount = 50;
            }
            experience += (int)amount;
            totalScore += (int)amount;
        }

        public void TakeDamage(float damage)
        {
            float leftoverDamage = 0;
            //Take the most damage possible out of shields, then take the remainder from energy.

            if (currentShieldAmount > 0) //we have sheld to use
            {
                currentShieldAmount -= damage;
                if (currentShieldAmount < 0) //Check to see if sheld couldn't fully take the hit
                {
                    leftoverDamage = Math.Abs(currentShieldAmount);
                    currentEnergyAmount -= leftoverDamage;
                }
            }
            else                         //no sheld 4 u
                currentEnergyAmount -= damage;

            if (currentEnergyAmount < 0)
                currentEnergyAmount = 0;
        }

        private void Initialize()
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (storage.FileExists("save.txt"))
                {
                    using (IsolatedStorageFileStream savefile = new IsolatedStorageFileStream("save.txt", FileMode.OpenOrCreate, storage))
                    {
                        using (BinaryReader reader = new BinaryReader(savefile))
                        {
                            currentChapter = reader.ReadInt32();
                            currentWorld = reader.ReadInt32();
                            unlocked = reader.ReadInt32();
                            Game1.ShouldPlayMusic = reader.ReadBoolean();
                            Game1.ShouldPlaySound = reader.ReadBoolean();
                            for (int x = 0; x < 6; x++)
                                for (int y = 0; y < 4; y++)
                                    medals[x][y] = reader.ReadInt32();
                        }
                    }
                }
            }
        }

        public void Save()
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())// grab the storage
            {
                using (FileStream stream = storage.OpenFile("save.txt", FileMode.Create))// Open a file in Create mode
                {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        //Save Game State
                        writer.Write(currentChapter);
                        writer.Write(currentWorld);
                        writer.Write(unlocked);
                        writer.Write(Game1.ShouldPlayMusic);
                        writer.Write(Game1.ShouldPlaySound);
                        //Save Medals
                        for (int x = 0; x < 6; x++)
                            for (int y = 0; y < 4; y++)
                                 writer.Write(medals[x][y]);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the number of medals earned on whichever level the player has just finished
        /// </summary>
        /// <returns># of medals for current world</returns>
        public int GetMedals()
        {
            return medals[currentWorld][currentChapter - (4 * currentWorld) - 1];
        }

        public void GiveMedals(int count)
        {
            medals[currentWorld][currentChapter - (4 * currentWorld) - 1] = count;
        }

        /// <summary>
        /// Returns an int representing the total number of medals earned
        /// </summary>
        /// <returns></returns>
        public int TotalMedalCount()
        {
            int medalCount = 0;
            for (int x = 0; x < 6; x++)
                for (int y = 0; y < 4; y++)
                    medalCount += medals[x][y];

            return medalCount;
        }

        public List<int> WorldMedals()
        {
            List<int> medalList = new List<int>();
            medalList.Add(medals[currentWorld][0]);
            medalList.Add(medals[currentWorld][1]);
            medalList.Add(medals[currentWorld][2]);
            medalList.Add(medals[currentWorld][3]);
            return medalList;
        }

        /// <summary>
        /// If each level in the world has a gold medal, returns true
        /// </summary>
        /// <returns>true if all gold, else false</returns>
        public bool IsWorldGold()
        {
            return ((medals[currentWorld][0] + medals[currentWorld][1] + medals[currentWorld][2] + medals[currentWorld][3]) == 12);
        }

        private void LevelUp(GameTime gameTime)
        {
            if (playerLevel < Game1.LevelCap)
            {
                playerLevel++;
                if (playerLevel < Game1.LevelCap)
                    drawLevelUp = true;
            }
            else if (playerLevel > Game1.LevelCap)
            {  
                playerLevel = Game1.LevelCap;
            }
            toNextLevel += (baseExpReq * playerLevel ^ 2);
        }

        public void Reset()
        {
            //Reset the player's gameplay values
            playerLevel = 1;
            experience = 0;
            totalScore = 0;
            combo = 1;
            currentEnergyAmount = 25;
            currentShieldAmount = 0;
            toNextLevel = baseExpReq;
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            if (drawLevelUp)
            {
                Color textColor;
                if (playerLevel == 2)
                    textColor = Color.Orange;
                else if (playerLevel == 3)
                    textColor = Color.Yellow;
                else if (playerLevel == 4)
                    textColor = Color.Lime;
                else
                    textColor = Color.Aqua;
                string text = "LEVEL UP";
                Vector2 textLoc = new Vector2(Game1.ScreenWidth / 2 - font.MeasureString(text).X / 2,
                                              Game1.ScreenHeight / 2 - font.MeasureString(text).Y / 2);
                spriteBatch.DrawString(font, text, textLoc, textColor);
            }
        }

        #region Accessors
        public float CurrentEnergyAmount
        {
            get { return currentEnergyAmount; }
            set { currentEnergyAmount = value; }
        }

        public float CurrentShieldAmount
        {
            get { return currentShieldAmount; }
            set { currentShieldAmount = value; }
        }

        public int Combo
        {
            get { return combo; }
            set { combo = value; }
        }

        public int PlayerLevel
        {
            get { return playerLevel; }
            set { playerLevel = value; }
        }

        public int UnlockedLevels
        {
            get { return unlocked; }
            set { unlocked = value; }
        }

        public int Experience
        {
            get { return experience; }
            set { experience = value; }
        }

        public int TotalScore
        {
            get { return totalScore; }
            set { totalScore = value; }
        }

        public int CurrentScore
        {
            get { return currentScore; }
            set { currentScore = value; }
        }

        public int CurrentChapter
        {
            get { return currentChapter; }
            set { currentChapter = value; }
        }

        public int CurrentWorld
        {
            get { return currentWorld; }
            set { currentWorld = value; }
        }

        public int LevelRequirement
        {
            get { return levelRequirement; }
            set { levelRequirement = value; }
        }

        #endregion
    }
}
