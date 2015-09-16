using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flake.iCalMaker
{
    public class turnierPlan
    {
        public SortedSet<DateTime> Tage { get; private set; }

        public string Description { get; set; }

        public turnierPlan(DateTime firstDay)
        {
            Tage = new SortedSet<DateTime>() { firstDay };
            Description = string.Empty;
        }

        public void AddDat(DateTime day)
        {
            Tage.Add(day);
        }

        public DateTime GetStartDay()
        {
            return Tage.First();
        }

        public DateTime GetEndDay()
        {
            return Tage.Last().AddDays(1);
        }
    }
}