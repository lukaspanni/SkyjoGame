using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Model
{
    class UserPlayer : Player
    {
        public string Name { get; set; }

        public UserPlayer(string name) : base(null, null)
        {
            Name = name;
        }
    }
}
