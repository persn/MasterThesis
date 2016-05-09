﻿using System.IO;

namespace NTNU.TestGame
{
    using System;

    public class TaskLoader
    {
        private string _folder;
        private string[] _3PlayerTasks;
        private string[] _4PlayerTasks;
        private string[] _5PlayerTasks;
        private string[] _6PlayerTasks;
        private readonly Random _random = new Random();

        public TaskLoader()
        {
            _folder = Directory.GetCurrentDirectory() + "../../../../../../../TestGame/tasks/";
            ReadTasksFromFiles();
        }

        private void ReadTasksFromFiles()
        {
            //TODO: Read from actual file(s)
            _3PlayerTasks = System.IO.File.ReadAllLines(_folder + "3playertasks.txt");
            _4PlayerTasks = System.IO.File.ReadAllLines(_folder + "4playertasks.txt");
            _5PlayerTasks = System.IO.File.ReadAllLines(_folder + "5playertasks.txt");
            _6PlayerTasks = System.IO.File.ReadAllLines(_folder + "6playertasks.txt");
        }

        public string LoadTask(int numPlayers)
        {
            switch (numPlayers)
            {
                case 3:
                    return _3PlayerTasks[_random.Next(_3PlayerTasks.Length)];
                case 4:
                    return _4PlayerTasks[_random.Next(_4PlayerTasks.Length)];
                case 5:
                    return _5PlayerTasks[_random.Next(_5PlayerTasks.Length)];
                case 6:
                    return _6PlayerTasks[_random.Next(_6PlayerTasks.Length)];
                default:
                    throw new NotSupportedException("Invalid number of players");
            }
            
        }
    }
}