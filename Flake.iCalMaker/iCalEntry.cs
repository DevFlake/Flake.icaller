using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flake.iCalMaker
{
    public class iCalEntry
    {
        /// <summary>
        /// 2.0 (?)
        /// </summary>
        public string VERSION { get; set; }

        /// <summary>
        /// eg PRODID:http://www.example.com/calendarapplication/
        /// </summary>
        public string PRODID { get; set; }

        /// <summary>
        /// PUBLISH (?)
        /// </summary>
        public string METHOD { get; set; }

        public List<iCalEvent> Events { get; set; }

        private string _uidSuffix = string.Empty;

        public iCalEntry(string priod, string uidSuffix)
        {
            Events = new List<iCalEvent>();
            VERSION = "2.0";
            PRODID = priod;
            METHOD = "PUBLISH";
            _uidSuffix = uidSuffix;
        }

        public string GetiCalContent()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("BEGIN:VCALENDAR").Append(Environment.NewLine);
            sb.Append("VERSION:").Append(VERSION).Append(Environment.NewLine);
            sb.Append("PRIOD:").Append(PRODID).Append(Environment.NewLine);
            sb.Append("METHOD:").Append(METHOD).Append(Environment.NewLine);

            foreach (var evt in Events)
            {
                sb.Append("BEGIN:VEVENT").Append(Environment.NewLine);
                sb.Append("UID:").Append(evt.UID).Append(_uidSuffix).Append(Environment.NewLine);
                if (evt.UseOrgnizer) sb.Append(evt.GetOrganizerLineForICal()).Append(Environment.NewLine);
                sb.Append("LOCATION:").Append(evt.LOCATION).Append(Environment.NewLine);

                sb.Append("SUMMARY:").Append(evt.SUMMARY).Append(Environment.NewLine);
                sb.Append("DESCRIPTION:").Append(evt.DESCRIPTION).Append(Environment.NewLine);
                sb.Append("CLASS:").Append(evt.CLASS).Append(Environment.NewLine);
                sb.Append("DTSTART:").Append(evt.DTSTART).Append(Environment.NewLine);
                sb.Append("DTEND:").Append(evt.DTEND).Append(Environment.NewLine);
                sb.Append("DTSTAMP:").Append(evt.DTSTAMP).Append(Environment.NewLine);
                sb.Append("END:VEVENT").Append(Environment.NewLine);
            }
            sb.Append("END:VCALENDAR");

            return sb.ToString();
        }
    }
}