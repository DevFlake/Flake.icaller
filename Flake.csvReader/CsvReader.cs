using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flake.csvReader
{
    public class CsvReader
    {
        public char FieldSeparator { get; set; }

        private string _rawData;

        public CsvReader(string csvData)
        {
            FieldSeparator = ';';
            _rawData = csvData;
        }

        public IEnumerable<string[]> GetTableData()
        {
            return GetRawData(_rawData);
        }

        private IEnumerable<string[]> GetRawData(string source)
        {
            List<string[]> ret = new List<string[]>();

            char[] charArray = { (char)13, (char)10 };
            foreach (string line in source.Split(charArray, StringSplitOptions.None))
            {
                string[] fields = { };
                try
                {
                    if (!line.ToLower().StartsWith("sep=") && !line.ToLower().StartsWith("dec="))
                    {
                        fields = line.Split(new[] { FieldSeparator });
                        for (int i = 0; i < fields.Length - 1; i++)
                        {
                            RemoveSpecialChars(ref fields[i]);
                        }
                    }
                    else
                    {
                        if (line.ToLower().StartsWith("sep=")) ReplaceSeparator(line.ToLower());
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    ex.ToString();

                    // TODO
                }
                ret.Add(fields);
            }
            return ret;
        }

        private Boolean ReplaceSeparator(string line)
        {
            char temp = FieldSeparator;
            try
            {
                if (line.StartsWith("sep=") && line.Length > 4)
                {
                    temp = line[4];
                    FieldSeparator = temp;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();

                // TODO
                return false;
            }
            return true;
        }

        private void RemoveSpecialChars(ref string source)
        {
            if ((source.Contains((char)10) && source.Contains((char)13)))
            {
                source = source.Replace(((char)10).ToString(), string.Empty).Replace("\"", "");
            }
            else
            {
                source = source.Replace((char)13, (char)10).Replace("\"", "");
            }
        }
    }
}