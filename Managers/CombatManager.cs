using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static RPGGame.ConsoleManager;
using static RPGGame.GlobalConstants;
using static RPGGame.GlobalVariables;
using static RPGGame.TextManager;

namespace RPGGame
{
    internal static class CombatManager
    {
        //Checks for aggressors in the list of entities provided. Returns true if they are defeated, false if they are not.
        public static bool CombatCheck(List<Entity> entList)
        {
            if (entList == null)
                return true;
            if (!entList.Exists(x => (x.Aggressive == true)))
                return true;
            List<Entity> enemyList = entList.FindAll(x => (x.Aggressive == true));
            WriteLine("Ahead you see " + enemyList.Count + " threats...");
            foreach (Entity enemy in enemyList)                                             //For each enemy
                if (!Combat(enemy))                                                         //Run a combat sequence
                    return false;                                                           //Fail if you fail combat
            return true;
        }
        
        //This is a combat sequence! The combat is run off of stats + rng + input commands to control the distance of the combat. It's simplistic but there are some aspects
        //of the system here I really like and think are unique, but I won't go into detail. But I may very well reuse a more robust and satisfying version of this system in
        //a future project. I also think the attack system could be restructured so that it's a single method, rather than a block almost identically repeated by each combatant.
        public static bool Combat(Entity enemy)
        {
            #region Setup
            int playerHealth = Player.Stats[CurrHealth];
            int enemyHealth = enemy.Stats[CurrHealth];

            List<Weapon> playerWeapons = Player.GetEquippedWeapons();
            List<Weapon> enemyWeapons = enemy.GetEquippedWeapons();

            int playerDefence = Player.GetDefence();
            int enemyDefence = enemy.GetDefence();

            int playerArmour = Player.GetArmour();
            int enemyArmour = enemy.GetArmour();

            int playerSpeed = Player.GetSpeed();
            int enemySpeed = enemy.GetSpeed();

            int playerMaxRange = Player.GetMaxRange();
            int enemyMaxRange = enemy.GetMaxRange();

            int enemyMinRange = enemy.GetMinRange();

            int distance = Math.Max(playerMaxRange, enemyMaxRange) + 1;
            #endregion

            WriteLine("Do you ADVANCE, RETREAT, HOLD or FLEE ?");
            do
            {
                //These two lines calculate and list what items are usable for each combatant at the start of the round.
                List<Weapon> enemyInRangeWeapons = enemyWeapons.FindAll(x => (distance >= x.Get("minRange") && (distance <= x.Get("maxRange"))));
                List<Weapon> playerInRangeWeapons = playerWeapons.FindAll(x => (distance >= x.Get("minRange") && (distance <= x.Get("maxRange"))));

                //AttackPhase
                #region Player Attack
                var alreadyAttacked = new List<Weapon>();                           //Ensures multislotted items don't attack multiple times.
                int damage = 0;
                if ((playerInRangeWeapons.Count == 0) && (distance == 1))
                    playerInRangeWeapons.Add(Fist);                                 //Use fists if you are upclose without a weapon.

                foreach (Weapon currWeapon in playerInRangeWeapons)                 //For each weapon
                {
                    if (!alreadyAttacked.Contains(currWeapon))                      //If it hasn't already attacked (multislots)
                    {
                        damage += Roll(currWeapon, enemyDefence, enemyArmour);      //Add damage provided by a roll function based on the weapon, and the other's defence and armour.
                        alreadyAttacked.Add(currWeapon);                            //Add it to the attacked list.
                    }
                }

                if (playerInRangeWeapons.Count != 0)                                //If you're in range with ANYTHING
                {
                    if (damage > 0)                                                 //If you did damage
                    {
                        WriteLine("You hit " + enemy.Name + " for " + damage.ToString() + " damage!");  //List it
                        enemyHealth -= damage;                                                          //Apply it
                    }
                    else
                        WriteLine("You did no damage to " + enemy.Name);            //Or state you did no damage.
                }
                #endregion

                //This basically repeats the block above, but in the other direction. I don't like that repetition, but I'm out of time to work out a method for it.
#region EnemyAttack
                alreadyAttacked = new List<Weapon>();
                damage = 0;
                if ((enemyInRangeWeapons.Count == 0) && (distance == 1))
                    enemyInRangeWeapons.Add(Fist);

                foreach (Weapon currWeapon in enemyInRangeWeapons)
                {

                    if (!alreadyAttacked.Contains(currWeapon))
                    {
                        damage += Roll(currWeapon, playerDefence, playerArmour);
                        alreadyAttacked.Add(currWeapon);
                    }
                }

                if (enemyInRangeWeapons.Count != 0)
                {
                    if (damage > 0)
                    {
                        WriteLine(enemy.Name + " hit you for " + damage.ToString() + " damage!");
                        playerHealth -= damage;
                    }
                    else
                        WriteLine(enemy.Name + " did no damage to you!");
                }
#endregion

#region Health check
                if (!(playerHealth > 0))                            //If the player loses all his health
                {
                    if (InternalTesting || ExternalTesting)         //Unless it's a test
                    {
                        WriteLine("Can't due during a test!");
                    }
                    else
                    {
                        WriteLine("You die...");                    //Run a death+exist sequence.
                        WriteLine("Press any key to exit!");
                        ReadLine();
                        System.Environment.Exit(0);
                        return false;                               //Yes, this return is redundant, but it's futureproofing for when I don't want the exit anymore.
                    }
                }

                
                    if (!(enemyHealth > 0))                         //If the enemy loses all his health
                {
                    WriteLine("You defeated " + enemy.Name + "!");  //Write that you defeated the enemy
                    enemy.Die();                                    //Run the enemy's die sequence
                    break;                                          //End the combat sequence
                }
                #endregion


#region MovePhase
                WriteLine("The " + enemy.Name + " is " + distance.ToString() + " units away."); //List the distance

                Redraw();                                                                       //Redraw the map

                int dist;
                string inCommand;

                #region Automate during testing
                if (InternalTesting || ExternalTesting)
                {
                    inCommand = "";
                    WriteLine("HOLD");
                    System.Threading.Thread.Sleep(500);
                }
                #endregion
                else
                    inCommand = ReadLine();                                           //Get user input

                switch (inCommand)
                {
                    case var someVal
                    when new Regex("ADVANCE \\d").IsMatch(someVal):                   //If it reads "ADVANCE" + a number, advance that number   
                        dist = int.Parse(someVal.Replace("ADVANCE ", ""));            
                        distance -= dist;
                        break;
                    case "ADVANCE":
                        distance -= playerSpeed;                                      //If it just says "ADVANCE", advance the maximum.
                        break;
                    case var someVal when new Regex("RETREAT \\d").IsMatch(someVal):  //If it reads "RETREAT" + a number, retreat that number.
                        dist = int.Parse(someVal.Replace("RETREAT ", ""));
                        distance += dist;
                        break;
                    case "RETREAT":                                                   //If it just says "RETREAT", retreat that number.
                        distance += playerSpeed;
                        break;
                    case "HOLD":                                                      //If it says "HOLD", do nothing.
                        break;
                    case "":                                                          //If it is empty, do nothing. (Need a better system, typos are punishing!)
                        break;
                    case "FLEE":                                                      //If it says "FLEE" and you're out of enemy range
                        if (distance > enemyMaxRange)
                        {
                            Player.SetHealth(playerHealth);                           //Set player health
                            enemy.SetHealth(enemyHealth);                             //Set enemy health
                            return false;                                             //Return a failed combat (And thus move)
                        }
                        else
                        {
                            distance += playerSpeed;                                  //Otherwise retreat your max distance
                            WriteLine("Too close to flee!");                          //And say that you're too close to flee
                            break;
                        }
                    case "QUIT":                                                      //If it says "QUIT"
                        WriteLine("Warning! Progress will be lost if you quit during combat! Type \"YES\" to confirm!");
                        if (ExternalTesting)                                          //Bypass user input during testing
                            playerHealth = 0;
                        else
                            if (ReadLine() == "YES")
                            playerHealth = 0;
                        break;
                    default:
                        WriteLine("Invalid command!");                                //If it is anything else, write that it is an invalid command. Again, punishing typos.
                        break;
                }

                if (enemyMaxRange < distance)                                         //If the enemy is too far to attack
                {
                    WriteLine("The " + enemy.Name + " advances!");                    //It advances
                    distance -= enemySpeed;
                }
                else if (enemyMinRange > distance)                                    //Else if the enemy is too close to attack
                {
                    WriteLine("The " + enemy.Name + " retreats!");                    //It retreats
                    distance += enemySpeed;
                }
                #endregion                                                         //Note that this could cause issues as it's possible to go from beyond maxrange to
                //within minrange. Not good, need a fix.

                distance = Math.Max(1, distance);                                     //Ensure distance doesn't go below 1.
            }
            while (playerHealth > 0 && enemyHealth > 0);                              //Repeat until someone has no health

            Player.SetHealth(playerHealth);                                           //Set the healths in the overworld
            enemy.SetHealth(enemyHealth);

            WriteLine("");                                                            //Add a space
            return true;                                                              //Return successful combat
        }

        private static int Roll(Weapon currWeapon, int defence, int armour)
        {
            var r = new Random();
            int weaponDamage = currWeapon.Get("damageModifier");                    //Get the weapon's damage
            int maxAttackRoll = currWeapon.Get("attackModifier");                   //Get the weapon's attack
            int attackRoll = r.Next(1, maxAttackRoll + 1);                          //Roll between 1 and attack
            int defenceRoll = r.Next(1, defence + 1);                               //Roll between 1 and defence

            if (attackRoll == maxAttackRoll)                                        //If an attack rolls it's max, it bypasses armour and returns its damage.
                return weaponDamage;

            if (attackRoll > defenceRoll)                                           //If an attack beats defence but doesn't hit max, it returns damage reduced by armour
                return weaponDamage - armour;

            return 0;                                                               //If it fails to either crit or beat armour, it returns zero
        }
    }
}
