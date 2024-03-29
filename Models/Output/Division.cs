﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBAllStar.Models.Output
{
    public class Division
    {
        public Division()
        {
            Teams = new List<Team>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public List<Team> Teams { get; set; }
        public int TotalTeamValue { get; set; }
    }
}
