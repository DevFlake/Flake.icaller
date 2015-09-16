using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flake.iCalMaker
{
    public class iCalEvent
    {
        /// <summary>
        /// eg UID:461092315540@example.com
        /// </summary>
        public string UID { get; private set; }

        /// <summary>
        /// eg ORGANIZER;CN="Alice Balder, Example Inc.":MAILTO:alice@example.com
        /// </summary>
        //public string ORGANIZER { get { return } }

        public string OrganizerName { get; set; }

        public string OrganizerMail { get; set; }

        public string LOCATION { get; set; }

        public string SUMMARY { get; set; }

        public string DESCRIPTION { get; set; }

        /// <summary>
        /// eg PUBLIC
        /// </summary>
        public string CLASS { get; set; }

        public string DTSTART { get; set; }

        public string DTEND { get; set; }

        /// <summary>
        /// creationdate
        /// </summary>
        public string DTSTAMP { get; set; }

        public bool UseOrgnizer { get { return (OrganizerMail != string.Empty); } }

        public iCalEvent()
        {
            UID = Guid.NewGuid().ToString();
        }

        public string GetOrganizerLineForICal()
        {
            string ret = "ORGANIZER";

            if (!string.IsNullOrEmpty(OrganizerName))
            {
                ret += ";CN=";
                ret += OrganizerName;
            }
            ret += ":mailto:";
            ret += OrganizerMail;
            return ret;
        }

        public iCalEvent Clone()
        {
            iCalEvent ret = new iCalEvent()
            {
                UID = Guid.NewGuid().ToString(),
                CLASS = CLASS,
                DESCRIPTION = DESCRIPTION,
                LOCATION = LOCATION,
                SUMMARY = SUMMARY,
                DTEND = DTEND,
                DTSTAMP = DTSTAMP,
                DTSTART = DTSTART,
                OrganizerMail = OrganizerMail,
                OrganizerName = OrganizerName
            };
            return ret;
        }
    }
}