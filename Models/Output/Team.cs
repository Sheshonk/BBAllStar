using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBAllStar.Models.Output
{
    public class Team
    {
        public Team()
        {
            AllStars = new List<Player>();
            Players = new List<Player>();
        }

        public List<Player> AllStars { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Player> Players { get; set; }
        public string Race { get; set; }
        public string Username { get; set; }
    }
}
