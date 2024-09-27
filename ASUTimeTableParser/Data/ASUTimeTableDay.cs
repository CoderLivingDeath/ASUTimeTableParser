using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASUTimeTableParser.Data
{
    public readonly struct ParsedASUScheduleTableClass
    {
        public readonly string Date;
        public readonly string Number;
        public readonly string Time;
        public readonly string Object;
        public readonly string Lecturer;
        public readonly string Audience;

        public ParsedASUScheduleTableClass(string date, string number, string time, string @object, string lecturer, string audience)
        {
            Date = date;

            Number = number;
            Time = time;
            Object = @object;
            Lecturer = lecturer;
            Audience = audience;
        }
    }
}
