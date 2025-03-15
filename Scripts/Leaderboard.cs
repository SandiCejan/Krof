using KrofEngine;
using System;
using System.Collections.Generic;

namespace Krof
{
    internal class Leaderboard : ISaveFile
    {
        public List<int> width = new();
        public List<int> height = new();
        public List<int> seed = new();
        public List<string> username = new();
        public List<TimeOnly> time = new();
        public List<GameMode> gameMode = new();
    }
}
