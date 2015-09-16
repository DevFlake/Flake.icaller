using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Flake.iCalMaker
{
    public class MakeiCal
    {
        private IEnumerable<string[]> _data;

        public MakeiCal(IEnumerable<string[]> tableData)
        {
            _data = tableData;
        }

        public string GetiCal()
        {
            string ret = string.Empty;
            ret = HandleTable(_data).GetiCalContent();
            return ret;
        }

        public double StandardDuration { get; set; }

        public string HomeLocation { get; set; }

        public bool GetTurniere { get; set; }

        public bool GetTraining { get; set; }

        public bool GetTurkoTraining { get; set; }

        private string _nameFilter = string.Empty;

        public string NameFilter { get { return _nameFilter; } set { _nameFilter = value.ToLower(); } }

        private string _teamFilter = string.Empty;

        public string TeamFilter { get { return _teamFilter; } set { _teamFilter = value.ToLower(); } }

        public string CalendarLocation { get; set; }

        public string UIDSuffix { get; set; }

        private iCalEntry HandleTable(IEnumerable<string[]> table)
        {
            iCalEntry ret = new iCalEntry(CalendarLocation, UIDSuffix); // TODO
            string orgMailString = ""; // "test@test.de"; // TODO
            string orgNameString = "tester test"; // TODO

            DateTime? runningDate = null;
            string runningLineDesc = string.Empty;
            bool isTrainig = false;
            bool isTurkoTrainig = false;
            bool isTurnier = false;
            bool sonstnurnochTraining = false;

            Dictionary<string, turnierPlan> turniere = new Dictionary<string, turnierPlan>();

            List<DateTime> trainingsWritten = new List<DateTime>();

            foreach (var line in table)
            {
                sonstnurnochTraining = line.Length > 4 && (line[4].StartsWith("Mo") || line[4].StartsWith("Di") ||
                    line[4].StartsWith("Mi") || line[4].StartsWith("Do") ||
                    line[4].StartsWith("Fr") || line[4].StartsWith("Sa") ||
                    line[4].StartsWith("So"));

                if (sonstnurnochTraining)
                {
                    iCalEvent training1 = new iCalEvent()
                    {
                        CLASS = "PUBLIC",
                        OrganizerMail = orgMailString,
                        OrganizerName = orgNameString,
                        DTSTAMP = Formatdate(DateTime.Now)
                    };

                    runningDate = ConvertDateTimeString(line[1], CultureInfo.CurrentCulture, DateTimeKind.Local, "", false);

                    training1.DTSTAMP = Formatdate(DateTime.Now);
                    if (line[0].StartsWith("Mi"))
                    {
                        training1.DTSTART = Formatdate(runningDate.Value.AddHours(17));
                        training1.DTEND = Formatdate(runningDate.Value.AddHours(22));
                    }
                    if (line[0].StartsWith("Fr"))
                    {
                        training1.DTSTART = Formatdate(runningDate.Value.AddHours(17));
                        training1.DTEND = Formatdate(runningDate.Value.AddHours(22));
                    }
                    if (line[0].StartsWith("Sa"))
                    {
                        training1.DTSTART = Formatdate(runningDate.Value.AddHours(14));
                        training1.DTEND = Formatdate(runningDate.Value.AddHours(17));
                    }
                    training1.SUMMARY = "Training";
                    training1.LOCATION = HomeLocation;
                    training1.DESCRIPTION = GetSonstigesTrainingDescriptionFromLine(line);

                    bool imp = GetTraining && (_nameFilter == string.Empty || training1.DESCRIPTION.ToLower().Contains(_nameFilter));

                    if (training1.DESCRIPTION.ToLower().Contains("turko"))
                    {
                        // TurkoTrainig!
                        training1.DTSTART = Formatdate(runningDate.Value.AddHours(9));
                        training1.DTEND = Formatdate(runningDate.Value.AddHours(17));
                        training1.SUMMARY = "Training mit Turko";
                        training1.DESCRIPTION = string.Empty;
                        imp = GetTurkoTraining;
                    }

                    if (imp && !string.IsNullOrEmpty(training1.SUMMARY) && !training1.DESCRIPTION.ToLower().Contains("kein")) ret.Events.Add(training1);
                    trainingsWritten.Add(runningDate.Value);

                    if (line.Length > 4)
                    {
                        iCalEvent training2 = new iCalEvent()
                        {
                            CLASS = "PUBLIC",
                            OrganizerMail = orgMailString,
                            OrganizerName = orgNameString,
                            DTSTAMP = Formatdate(DateTime.Now)
                        };

                        runningDate = ConvertDateTimeString(line[5].Contains(" ") ? line[5].Split(new char[] { ' ' })[0] : line[5], CultureInfo.CurrentCulture, DateTimeKind.Local, "", false);

                        training2.DTSTAMP = Formatdate(DateTime.Now);
                        if (line[0].StartsWith("Mi"))
                        {
                            training2.DTSTART = Formatdate(runningDate.Value.AddHours(17));
                            training2.DTEND = Formatdate(runningDate.Value.AddHours(22));
                        }
                        if (line[0].StartsWith("Fr"))
                        {
                            training2.DTSTART = Formatdate(runningDate.Value.AddHours(17));
                            training2.DTEND = Formatdate(runningDate.Value.AddHours(22));
                        }
                        if (line[0].StartsWith("Sa"))
                        {
                            training2.DTSTART = Formatdate(runningDate.Value.AddHours(14));
                            training2.DTEND = Formatdate(runningDate.Value.AddHours(17));
                        }
                        training2.SUMMARY = "Training";
                        training2.LOCATION = HomeLocation;
                        training2.DESCRIPTION = GetSonstigesTrainingDescriptionFromLine(line, true);

                        imp = GetTraining && (_nameFilter == string.Empty || training2.DESCRIPTION.ToLower().Contains(_nameFilter));

                        if (training2.DESCRIPTION.ToLower().Contains("turko"))
                        {
                            // TurkoTrainig!
                            training2.DTSTART = Formatdate(runningDate.Value.AddHours(9));
                            training2.DTEND = Formatdate(runningDate.Value.AddHours(17));
                            training2.SUMMARY = "Training mit Turko";
                            training2.DESCRIPTION = string.Empty;
                            imp = GetTurkoTraining;
                        }

                        if (imp && !string.IsNullOrEmpty(training2.SUMMARY) && !training2.DESCRIPTION.ToLower().Contains("kein")) ret.Events.Add(training2);
                        trainingsWritten.Add(runningDate.Value);
                    }

                    //string aufsicht = line[6].Contains(" ") ? line[6].Split(new char[] { ' ' })[1] : line[6];

                    continue;
                }

                if (line[0].StartsWith("Mo") || line[0].StartsWith("Di") ||
                    line[0].StartsWith("Mi") || line[0].StartsWith("Do") ||
                    line[0].StartsWith("Fr") || line[0].StartsWith("Sa") ||
                    line[0].StartsWith("So"))
                {
                    // neuer Tageseintrag
                    runningDate = ConvertDateTimeString(line[1], CultureInfo.CurrentCulture, DateTimeKind.Local, "", false);
                    runningLineDesc = line[2];
                    isTrainig = runningLineDesc.ToLower().Contains("training") && !runningLineDesc.ToLower().Contains("turko") &&
                          !runningLineDesc.ToLower().Contains("kein");
                    isTurkoTrainig = runningLineDesc.ToLower().Contains("turko");
                    isTurnier = runningLineDesc.ToLower().Contains("turnier");

                    DateTime? tempTime = ConvertDateTimeString(line[7], CultureInfo.CurrentCulture, DateTimeKind.Local, "", false);

                    if (runningDate.HasValue)
                    {
                        iCalEvent temp = new iCalEvent()
                        {
                            CLASS = "PUBLIC",
                            OrganizerMail = orgMailString,
                            OrganizerName = orgNameString,
                            DTSTAMP = Formatdate(DateTime.Now)
                        };

                        // EVENT
                        // Class, uid,organizer, dtstam schon gesetzt
                        // DTStart
                        DateTime tempStart = runningDate.Value;
                        if (tempTime.HasValue)
                        {
                            tempStart = tempStart.AddHours(tempTime.Value.Hour).AddMinutes(tempTime.Value.Minute);
                        }
                        temp.DTSTART = Formatdate(tempStart);

                        //DTend
                        temp.DTEND = Formatdate(tempStart.AddHours(StandardDuration));

                        // Location
                        temp.LOCATION = GetLocationFromLine(line);

                        // kurzinfo
                        temp.SUMMARY = GetSummaryFromLine(line);

                        // beschreibung
                        temp.DESCRIPTION = GetDescriptionFromLine(line);

                        if ((_teamFilter == string.Empty || temp.DESCRIPTION.ToLower().Contains(_teamFilter)) && !string.IsNullOrEmpty(temp.SUMMARY)) ret.Events.Add(temp); //ADDER

                        if (isTrainig && !trainingsWritten.Contains(runningDate.Value))
                        {
                            iCalEvent training = temp.Clone();

                            training.DTSTAMP = Formatdate(DateTime.Now);
                            if (line[0].StartsWith("Mi"))
                            {
                                training.DTSTART = Formatdate(runningDate.Value.AddHours(17));
                                training.DTEND = Formatdate(runningDate.Value.AddHours(22));
                            }
                            if (line[0].StartsWith("Fr"))
                            {
                                training.DTSTART = Formatdate(runningDate.Value.AddHours(17));
                                training.DTEND = Formatdate(runningDate.Value.AddHours(22));
                            }
                            if (line[0].StartsWith("Sa"))
                            {
                                training.DTSTART = Formatdate(runningDate.Value.AddHours(14));
                                training.DTEND = Formatdate(runningDate.Value.AddHours(17));
                            }
                            training.SUMMARY = "Training";
                            training.LOCATION = HomeLocation;
                            training.DESCRIPTION = GetTrainingDescriptionFromLine(line);

                            if (GetTraining && (_nameFilter == string.Empty || temp.DESCRIPTION.ToLower().Contains(_nameFilter)) && !string.IsNullOrEmpty(training.SUMMARY)) ret.Events.Add(training); //ADDER
                            trainingsWritten.Add(runningDate.Value);
                        }
                        if (isTurkoTrainig)
                        {
                            iCalEvent training = temp.Clone();
                            training.DTSTAMP = Formatdate(DateTime.Now);
                            training.DTSTART = Formatdate(runningDate.Value.AddHours(9));
                            training.DTEND = Formatdate(runningDate.Value.AddHours(17));
                            training.SUMMARY = "Training mit Turko";
                            training.LOCATION = HomeLocation;
                            training.DESCRIPTION = string.Empty;

                            if (GetTurkoTraining && !string.IsNullOrEmpty(training.SUMMARY)) ret.Events.Add(training);  //ADDER
                        }
                        if (isTurnier && line.Length > 3)
                        {
                            if (line.Length < 4 || (line.Length > 5 && string.IsNullOrEmpty(line[5])))
                            {
                                string fahrer = ReadFahrerForTurnier(line);

                                if (turniere.ContainsKey(line[3]))
                                {
                                    if (!turniere[line[3]].Tage.Contains(runningDate.Value))
                                    {
                                        turniere[line[3]].AddDat(runningDate.Value);
                                        if (!string.IsNullOrEmpty(fahrer) && string.IsNullOrEmpty(turniere[line[3]].Description))
                                            turniere[line[3]].Description = "Fahrer: " + fahrer;
                                    }
                                }
                                else
                                {
                                    turnierPlan tempturnierplan = new turnierPlan(runningDate.Value);
                                    if (!string.IsNullOrEmpty(fahrer)) tempturnierplan.Description = "Fahrer: " + fahrer;

                                    turniere.Add(line[3], tempturnierplan);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (line.Length > 2 && !string.IsNullOrEmpty(line[3]) && string.IsNullOrEmpty(line[0]) && !line[3].ToLower().Contains("terminplan"))
                    {
                        // neuer Eintrag am gleichen Tag

                        DateTime? tempTime = ConvertDateTimeString(line[7], CultureInfo.CurrentCulture, DateTimeKind.Local, "", false);

                        if (runningDate.HasValue)
                        {
                            iCalEvent temp = new iCalEvent()
                            {
                                CLASS = "PUBLIC",
                                OrganizerMail = orgMailString,
                                OrganizerName = orgNameString,
                                DTSTAMP = Formatdate(DateTime.Now)
                            };

                            // EVENT
                            // Class, uid,organizer, dtstam schon gesetzt
                            // DTStart
                            DateTime tempStart = runningDate.Value;
                            if (tempTime.HasValue)
                            {
                                tempStart = tempStart.AddHours(tempTime.Value.Hour).AddMinutes(tempTime.Value.Minute);
                            }
                            temp.DTSTART = Formatdate(tempStart);

                            //DTend
                            temp.DTEND = Formatdate(tempStart.AddHours(StandardDuration));

                            // Location
                            temp.LOCATION = GetLocationFromLine(line, runningLineDesc);

                            // kurzinfo
                            temp.SUMMARY = GetSummaryFromLine(line, runningLineDesc);

                            // beschreibung
                            temp.DESCRIPTION = GetDescriptionFromLine(line, runningLineDesc);

                            if ((_teamFilter == string.Empty || temp.DESCRIPTION.ToLower().Contains(_teamFilter)) && !string.IsNullOrEmpty(temp.SUMMARY)) ret.Events.Add(temp); //ADDER
                            if (isTurnier && line.Length > 3)
                            {
                                if (line.Length < 4 || (line.Length > 5 && string.IsNullOrEmpty(line[5])))
                                {
                                    string fahrer = ReadFahrerForTurnier(line);

                                    if (turniere.ContainsKey(line[3]))
                                    {
                                        if (!turniere[line[3]].Tage.Contains(runningDate.Value))
                                        {
                                            turniere[line[3]].AddDat(runningDate.Value);
                                            if (!string.IsNullOrEmpty(fahrer) && string.IsNullOrEmpty(turniere[line[3]].Description))
                                                turniere[line[3]].Description = "Fahrer: " + fahrer;
                                        }
                                    }
                                    else
                                    {
                                        turnierPlan tempturnierplan = new turnierPlan(runningDate.Value);
                                        if (!string.IsNullOrEmpty(fahrer)) tempturnierplan.Description = "Fahrer: " + fahrer;

                                        turniere.Add(line[3], tempturnierplan);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (GetTurniere)
            {
                foreach (var turnier in turniere)
                {
                    iCalEvent temp = new iCalEvent()
                    {
                        CLASS = "PUBLIC",
                        OrganizerMail = orgMailString,
                        OrganizerName = orgNameString,
                        DTSTAMP = Formatdate(DateTime.Now)
                    };

                    // EVENT
                    // Class, uid,organizer, dtstam schon gesetzt
                    // DTStart
                    temp.DTSTART = Formatdate(turnier.Value.GetStartDay(), false);

                    //DTend
                    temp.DTEND = Formatdate(turnier.Value.GetEndDay(), false);

                    // Location
                    temp.LOCATION = ""; // TODO

                    // kurzinfo
                    temp.SUMMARY = turnier.Key;

                    // beschreibung
                    temp.DESCRIPTION = turnier.Value.Description;

                    ret.Events.Add(temp);  //ADDER
                }
            }
            return ret;
        }

        /// <summary>
        /// convert datetime string to DateTime
        /// </summary>
        /// <param name="value">string datetiem value</param>
        /// <param name="dateTimeCulture">culture of datetime in string</param>
        /// <param name="dateTimeKind">kind of datetime in string</param>
        /// <param name="dateTimeFormat">format of datetime in string</param>
        /// <returns></returns>
        public static DateTime? ConvertDateTimeString(string value, CultureInfo dateTimeCulture, DateTimeKind dateTimeKind, string dateTimeFormat = "", bool convertDateTimeKind = true)
        {
            // TODO test if datestring has time component

            if (value == null)
            {
                // TODO StatsLog - WARN
                return null;
            }

            DateTime ret = DateTime.MinValue;
            DateTimeStyles dateTimeStyles = DateTimeStyles.AllowInnerWhite | DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowWhiteSpaces;
            DateTime temp = new DateTime();
            bool success = false;
            if (!string.IsNullOrEmpty(dateTimeFormat))
            {
                success = DateTime.TryParseExact(value, dateTimeFormat, dateTimeCulture, dateTimeStyles, out temp);
            }
            else
            {
                success = DateTime.TryParse(value, dateTimeCulture, dateTimeStyles, out temp);
            }

            if (success)
            {
                ret = new DateTime(temp.Ticks, dateTimeKind);
            }
            else
            {
                // TODO StatusLog - WARN
                return null;
            }

            return (convertDateTimeKind) ? ret.ToUniversalTime() : ret;
        }

        private string Formatdate(DateTime date, bool includeTime = true)
        {
            if (includeTime)
            {
                return date.ToUniversalTime().ToString("s").Replace(":", "").Replace("-", "") + "Z";
            }
            else
            {
                return date.ToString("yyyyMMdd");
            }
        }

        private string GetLocationFromLine(string[] line, string runningLineDesc = "")
        {
            if (line[2].ToLower().Contains("spiel") || runningLineDesc.ToLower().Contains("spiel"))
            {
                // Spiel
                if (line[3].StartsWith("HERREN") || line[3].StartsWith("DAMEN") || line[3].StartsWith("MÄDCHEN") || line[3].StartsWith("JUNGEN"))
                {
                    // heimspiel
                    return HomeLocation;
                }
                else
                {
                    if (line[5].StartsWith("HERREN") || line[5].StartsWith("DAMEN") || line[5].StartsWith("MÄDCHEN") || line[5].StartsWith("JUNGEN"))
                    {
                        // auswärtsspiel
                        return "bei " + line[3];
                    }
                }
            }
            return string.Empty;
        }

        private string GetSummaryFromLine(string[] line, string runningLineDesc = "")
        {
            if (line[2].ToLower().Contains("spiel") || runningLineDesc.ToLower().Contains("spiel"))
            {
                // Spiel
                if (line[3].StartsWith("HERREN") || line[3].StartsWith("DAMEN") || line[3].StartsWith("MÄDCHEN") || line[3].StartsWith("JUNGEN") ||
                    line[5].StartsWith("HERREN") || line[5].StartsWith("DAMEN") || line[5].StartsWith("MÄDCHEN") || line[5].StartsWith("JUNGEN"))
                {
                    // heimspiel oder auswärtsspiel
                    return "Spiel: " + line[3] + " geg. " + line[5];
                }
            }
            return string.Empty;
        }

        private string GetDescriptionFromLine(string[] line, string runningLineDesc = "")
        {
            string ret = GetSummaryFromLine(line, runningLineDesc);
            if (!string.IsNullOrEmpty(ret))
            {
                if (line.Length > 8 && !string.IsNullOrEmpty(line[8]))
                {
                    //ret += Environment.NewLine;
                    ret += "; Fahrer: " + line[8];
                }
                if (line.Length > 9 && !string.IsNullOrEmpty(line[8]))
                {
                    //ret += Environment.NewLine;
                    ret += "; Abfahrt: " + line[9];
                }
                if (line.Length > 10 && !string.IsNullOrEmpty(line[10]))
                {
                    //ret += Environment.NewLine;
                    ret += "; Aufsicht: " + line[10];
                }
            }
            return ret;
        }

        private string GetTrainingDescriptionFromLine(string[] line)
        {
            string ret = "Training";
            if (line.Length > 10 && !string.IsNullOrEmpty(line[10]))
            {
                //ret += Environment.NewLine;
                ret += "; Aufsicht: " + line[10];
            }
            return ret;
        }

        private string ReadFahrerForTurnier(string[] line)
        {
            if (line.Length > 9 && !string.IsNullOrEmpty(line[9]))
            {
                return line[9];
            }
            else
            {
                return string.Empty;
            }
        }

        private string GetSonstigesTrainingDescriptionFromLine(string[] line, bool zweiteSpalte = false)
        {
            string ret = "Training";
            if (zweiteSpalte)
            {
                if (line.Length > 5 && !string.IsNullOrEmpty(line[5]))
                {
                    string aufsicht = line[5].Contains(" ") ? GetCellTail(line[5].Split(new char[] { ' ' })) : line[6];
                    ret += "; Aufsicht: " + aufsicht;
                }
                if (!string.IsNullOrEmpty(line[5]) && line[5].Contains(" "))
                {
                    if (line.Length > 7 && !string.IsNullOrEmpty(line[7]))
                    {
                        ret += "; Sonstiges: " + line[7];
                    }
                }
                else
                {
                    if (line.Length > 6 && !string.IsNullOrEmpty(line[6]))
                    {
                        ret += "; Sonstiges: " + line[7];
                    }
                }
            }
            else
            {
                if (line.Length > 2 && !string.IsNullOrEmpty(line[2]))
                {
                    ret += "; Aufsicht: " + line[2];
                }
                if (line.Length > 3 && !string.IsNullOrEmpty(line[3]))
                {
                    ret += "; Sonstiges: " + line[3];
                }
            }
            return ret;
        }

        private string GetCellTail(string[] cell)
        {
            string ret = string.Empty;

            int i = 0;
            foreach (var s in cell)
            {
                if (i > 0)
                {
                    ret += s;
                }
                i++;
            }

            return ret.Trim();
        }
    }
}