using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace sselIndReports.AppCode
{
    public class ParsedFileName
    {
        private string _Name;
        private DateTime _AggStartDate;
        private DateTime _ReportDate;
        private int _Threshold;
        private IList<string> _Errors = new List<string>();

        private ParsedFileName(string fileName)
        {
            _Name = fileName;
            Parse();
        }

        public string Name
        {
            get{return _Name;}
        }

        public DateTime AggStartDate
        {
            get{return _AggStartDate;}
        }

        public DateTime ReportDate
        {
            get{return _ReportDate;}
        }

        public int Threshold
        {
            get{return _Threshold;}
        }

        public string[] Errors
        {
            get{return _Errors.ToArray();}
        }

        public static ParsedFileName Parse(string fileName)
        {
            return new ParsedFileName(fileName);
        }

        private void Parse()
        {
            string f = Path.GetFileNameWithoutExtension(_Name);
            string[] splitter = f.Split('_');
            if (splitter.Length == 3)
            {
                string temp,yy,mm;

                try
                {
                    temp = splitter[0];
                    temp = temp.Replace("AS", string.Empty);
                    yy = temp.Substring(0, 4);
                    mm = temp.Substring(4, 2);
                    _AggStartDate = new DateTime(Convert.ToInt32(yy), Convert.ToInt32(mm), 1);
                }
                catch (Exception ex)
                {
                    _Errors.Add(ex.Message);
                }

                try
                {
                    temp = splitter[1];
                    temp = temp.Replace("Report", string.Empty);
                    yy = temp.Substring(0, 4);
                    mm = temp.Substring(4, 2);
                    _ReportDate = new DateTime(Convert.ToInt32(yy), Convert.ToInt32(mm), 1);
                }
                catch (Exception ex)
                {
                    _Errors.Add(ex.Message);
                }

                try
                {
                    _Threshold = Convert.ToInt32(splitter[2]);
                }
                catch (Exception ex)
                {
                    _Errors.Add(ex.Message);
                }
            }
            else
                _Errors.Add("Incorrect file name format.");
        }
    }
}
