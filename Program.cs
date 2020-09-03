using System;
using System.IO;

namespace State_Machine
{
    class Program
    {
        string String;
        int te;
        int nt;

        static void Main(string[] args)
        {
            string str = File.ReadAllText("input.txt");
            
            StateMachine p = new StateMachine(str);
            Console.WriteLine("Current State = " + p.CurrentState);

            try
            {
                p.Go();
            }
            catch (Exception e) { Console.WriteLine(e.Message); }

            Console.ReadLine();
        }
    }
}
