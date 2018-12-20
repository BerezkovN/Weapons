using System;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace CSharp_Shell
{
    interface IWeapon
    {
        void Reload();
        void Shoot(ref bool check, int a);
        void Aim();
        void Info();
    }

    public class Weapon : IWeapon
    {
        Random rnd = new Random();    //I created Random to simulate hitting on target
        Random rnd2 = new Random();    //I can't use one random for three tasks
        Random rnd3 = new Random();
        public string Name;    //Name of weapon
        public int BoxMagazine;    //Constant value of max count of bullets in magazine
        public int AllBullets { get; set; }    //"bull"
        public int Num { get; set; }    //"num"
        public int CurrentBullets;    //Bullets on magazine, now
        public int unluck { get; set; }    //It isn't important. I did it to check how many does Random give percents less than 5
        protected bool aim = false;
        protected bool _aim;    //temp aim
        protected int chance = 5;

        public void Reload()
        {
            if (AllBullets > 0 && (CurrentBullets != BoxMagazine))    //If we have no any bullets, we can't reload
            {
                if (AllBullets >= BoxMagazine && CurrentBullets == 0)    //If all bullets are more than max count of bullets in box magazine, program will refull current bullets
                {
                    AllBullets -= BoxMagazine;
                    CurrentBullets = BoxMagazine;
                }
                else    //Else we set bullets what we need or what we can
                {
                    int num = BoxMagazine - CurrentBullets;
                    if (AllBullets > num)
                    {
                        CurrentBullets += num;
                        AllBullets -= num;
                    }
                    else
                    {
                        CurrentBullets += AllBullets;
                        AllBullets = 0;
                    }
                }
                _aim = false;    //This need to fix bags
                unluck = 0;
                Console.WriteLine("{0} has been reloaded\n", Name);
            }
            else if(CurrentBullets == BoxMagazine)
                Console.WriteLine("Magazine is full");
            else
                Console.WriteLine("You have not any bullets!!!");
        }

        public void Shoot(ref bool check, int a)
        {
            if (CurrentBullets > 0)    //If we have some bullets to shoot
            {
                CurrentBullets--;
                if (Num > 1 && _aim)    //If we have loop with aiming, we do this. We need to create temp because after shoot, aim sets to false
                {
                    aim = true;
                }

                Console.WriteLine("Result - {0}\n", Target(aim));    //Random
                if (a == Num - 1)    //Fixing bag
                    _aim = false;
                else
                    _aim = aim;
                aim = false;
            }
            else
            {
                Console.WriteLine("Your box magazine is empty!!!");
                check = true;
            }
        }

        public void Aim()
        {
            aim = true;
        }

        public void Info()
        {
            Console.WriteLine("Bullets - {0}\nBullets in box magazine - {1}\nUnluck - {2}\n", AllBullets, CurrentBullets, unluck);
        }

        public void Set()    //This void need to correctly set bullets
        {
            int SetBull = AllBullets >= BoxMagazine ? BoxMagazine : AllBullets;    //If all bullets are more than box magaxine sets max count of bullets in box magazine, else sets all bullets
            CurrentBullets = SetBull;
            AllBullets -= SetBull;
        }

        public int Target(bool aim)    //If we have aimed, our range of result will be better
        {
            int r = rnd.Next(0, 100);
            
            if (aim)
            {
                if (r >= chance) return rnd2.Next(8, 11);
                else {unluck++; return rnd3.Next(0,6);}
            }
            else
            {
                if (r >= chance) return rnd2.Next(0, 6);
                else {unluck++; return rnd3.Next(8, 11);}
            }
        }
    }
    //-------OUR WEAPONS-------//
    //You can write your weapons and properties
    public class Negev : Weapon
    {
        public Negev(int bull)
        {
            Name = "Negev";
            AllBullets = bull;
            BoxMagazine = 150;
            Set();
        }
    }
    
    public class M16 : Weapon
    {
        public M16(int bull)
        {
            Name = "M16";
            AllBullets = bull;
            BoxMagazine = 30;
            Set();
        }
    }

    public class Glock : Weapon
    {
        public Glock(int bull)
        {
            Name = "Glock";
            AllBullets = bull;
            BoxMagazine = 17;
            Set();
        }
    }

    public class AWP : Weapon
    {
        public AWP(int bull)
        {
            Name = "AWP";
            AllBullets = bull;
            BoxMagazine = 7;
            Set();
        }
    }
    //-------------------------//
    public static class Program
    {
        public static void Main()
        {
            List<int> res = new List<int>();
            string name = "CSharp_Shell.";    //this is name of namespace
            string s = null;    //with this variale we will work
            var child = Assembly.GetAssembly(typeof(Weapon))
            .GetTypes()
            .Where(el => el.IsSubclassOf(typeof(Weapon)));    //Getting all derived classes from Weapon
            Console.WriteLine("Choose weapon");
            for (int x = 0; x < child.ToArray().Length; x++)    //And output every class name in "child"
            {
                Console.Write(child.ToArray()[x].Name);
                if (x != child.ToArray().Length - 1)    //This need to avoid this "M16, " or "M16, Glock, "
                {
                    Console.Write(", ");
                }
            }
            Console.WriteLine();

            s = Console.ReadLine();    //Getting name of gun
            Console.Write("Enter count of bullets: ");
            int bull = Int32.Parse(Console.ReadLine());    //Getting count of bullets. Bullets in magazine don't count

            object o = Activator.CreateInstance(Type.GetType(name + s), bull);    //"bull" sets in constructor
            Weapon weapon = (Weapon)o;    //Creating class with name of class

            while (s != "End")    //This loop need to input commands
            {
                s = Console.ReadLine();
                int num = 1;    //I did this for comfort. If you want to shoot 10 times you can input "Shoot 10" instead "Shoot Shoot..."
                if (s.Contains("Shoot") && s.Length > "Shoot".Length)    //Getting number of repeats
                {
                    num = Int32.Parse(s.Substring("Shoot".Length + 1));
                    s = "Shoot";
                }
                switch (s)    //Reading commands
                {
                    case "Shoot":
                        bool check = false;    //This variable need to stop loop if magaxine has no bullets
                        weapon.Num = num;    //Setting value of Num
                        for (int a = 0; a < num; a++)    //Shooting
                        {
                            weapon.Shoot(ref check, a);
                            if (check)
                                break;
                        }
                        break;
                    case "Reload":
                        weapon.Reload();
                        break;
                    case "Aim":
                        weapon.Aim();
                        break;
                    case "Info":
                        weapon.Info();
                        break;
                    case "Clear":    //¯\_(ツ)_/¯
                        Console.Clear();
                        break;
                    case "Add":    //Adding bullets
                        int add = Int32.Parse(Console.ReadLine());
                        weapon.AllBullets += add;
                        Console.WriteLine("{0} {1} has been added to {2}", add, add!=1?"bullets":"bullet", weapon.Name);
                        break;
                    case "End":
                        goto l1;
                }
            }

            l1:
            Console.WriteLine("Program has been completed...");
            //Console.ReadKey();
        }
    }
}
//by Berezkov Nikita
//Thanks for reading
//And excuse me for grammar mistakes
//I'm working with my grammar
//Good Luck