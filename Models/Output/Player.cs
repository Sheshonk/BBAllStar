using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBAllStar.Models.Output
{
    public class Player
    {
        public Player()
        {
            SkillUps = new List<string>();
        }
        
        public int Id { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public string Position { get; set; }
        public List<string> SkillUps { get; set; }
        public int TeamValue { get; set; }
        public int TotalStarPlayerPoints { get; set; }
    }
}
