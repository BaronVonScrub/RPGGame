﻿using System;
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
        public static bool CombatCheck(List<Entity> entList)
        {
            if (entList == null)
                return true;
            if (!entList.Exists(x => (x.Aggressive == true)))
                return true;
            List<Entity> enemyList = entList.FindAll(x => (x.Aggressive == true));
            WriteLine("Ahead you see " + enemyList.Count + " threats...");
            foreach (Entity enemy in enemyList)
                if (!Combat(enemy))
                    return false;
            return true;
        }

        public static bool Combat(Entity enemy)
        {
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

            WriteLine("Do you ADVANCE, RETREAT, HOLD or FLEE ?");
            do
            {
                List<Weapon> enemyInRangeWeapons = enemyWeapons.FindAll(x => (distance >= x.Get("minRange") && (distance <= x.Get("maxRange"))));
                List<Weapon> playerInRangeWeapons = playerWeapons.FindAll(x => (distance >= x.Get("minRange") && (distance <= x.Get("maxRange"))));

                //AttackPhase
                //PlayerAttack
                var alreadyAttacked = new List<Weapon>();
                int damage = 0;
                if ((playerInRangeWeapons.Count == 0) && (distance == 1))
                    playerInRangeWeapons.Add(Fist);

                foreach (Weapon currWeapon in playerInRangeWeapons)
                {
                    if (!alreadyAttacked.Contains(currWeapon))
                    {
                        damage += Roll(currWeapon, enemyDefence, enemyArmour);
                        alreadyAttacked.Add(currWeapon);
                    }
                }

                if (playerInRangeWeapons.Count != 0)
                {
                    if (damage > 0)
                    {
                        WriteLine("You hit " + enemy.Name + " for " + damage.ToString() + " damage!");
                        enemyHealth -= damage;
                    }
                    else
                        WriteLine("You did no damage to " + enemy.Name);
                }

                //EnemyAttack
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

                if (!(playerHealth > 0))
                {
                    if (InternalTesting || ExternalTesting)
                    {
                        WriteLine("Can't due during a test!");
                    }
                    else
                    {
                        WriteLine("You die...");
                        WriteLine("Press any key to exit!");
                        ReadLine();
                        if (!ExternalTesting)
                            System.Environment.Exit(0);
                        return false;
                    }
                }

                    if (!(enemyHealth > 0))
                {
                    WriteLine("You defeated " + enemy.Name + "!");
                    enemy.Die();
                    break;
                }



                    //MovePhase
                    WriteLine("The " + enemy.Name + " is " + distance.ToString() + " units away.");

                Redraw();

                int dist;
                string inCommand;
                if (InternalTesting || ExternalTesting)
                {
                    inCommand = "";
                    WriteLine("HOLD");
                    System.Threading.Thread.Sleep(500);
                }
                else
                    inCommand = ReadLine();

                switch (inCommand)
                {
                    case var someVal
                    when new Regex("ADVANCE \\d").IsMatch(someVal):
                        dist = int.Parse(someVal.Replace("ADVANCE ", ""));
                        distance -= dist;
                        break;
                    case "ADVANCE":
                        distance -= playerSpeed;
                        break;
                    case var someVal when new Regex("RETREAT \\d").IsMatch(someVal):
                        dist = int.Parse(someVal.Replace("RETREAT ", ""));
                        distance += dist;
                        break;
                    case "RETREAT":
                        distance += playerSpeed;
                        break;
                    case "HOLD":
                        break;
                    case "":
                        break;
                    case "FLEE":
                        if (distance > enemyMaxRange)
                        {
                            Player.SetHealth(playerHealth);
                            enemy.SetHealth(enemyHealth);
                            return false;
                        }
                        else
                            WriteLine("Too close to flee!");
                        break;
                    case "QUIT":
                        WriteLine("Warning! Progress will be lost if you quit during combat! Type \"YES\" to confirm!");
                        if (ExternalTesting)
                            playerHealth = 0;
                        else
                            if (ReadLine() == "YES")
                            playerHealth = 0;
                        break;
                    default:
                        WriteLine("Invalid command!");
                        break;
                }

                if (enemyMaxRange < distance)
                {
                    WriteLine("The " + enemy.Name + " advances!");
                    distance -= enemySpeed;
                }
                else if (enemyMinRange > distance)
                {
                    WriteLine("The " + enemy.Name + " retreats!");
                    distance += enemySpeed;
                }

                distance = Math.Max(1, distance);


                distance = Math.Max(1, distance);
            }
            while (playerHealth > 0 && enemyHealth > 0);

            Player.SetHealth(playerHealth);
            enemy.SetHealth(enemyHealth);

            WriteLine("");
            return true;
        }

        private static int Roll(Weapon currWeapon, int defence, int armour)
        {
            var r = new Random();
            int weaponDamage = currWeapon.Get("damageModifier");
            int maxAttackRoll = currWeapon.Get("attackModifier");
            int attackRoll = r.Next(1, maxAttackRoll + 1);
            int defenceRoll = r.Next(1, defence + 1);

            if (attackRoll == maxAttackRoll)
                return weaponDamage;

            if (attackRoll > defenceRoll)
                return weaponDamage - armour;

            return 0;
        }
    }
}
