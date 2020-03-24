using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Model
{
    public class UserPlayer : Player
    {
        public string Name { get; set; }

        public UserPlayer(string name)
        {
            Name = name;
            Id = name;
        }
    }
}
