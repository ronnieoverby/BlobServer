using System;
using System.Collections.Generic;
using System.IO;
using CoreTechs.Common;

namespace BlobServer.Infrastructure
{
    public class DateTimePathCreator : IPathCreator
    {
        public DateTimePathCreator()
        {
            Resolution = DateTimePrecision.Minute;
        }

        public DateTimePrecision Resolution { get; set; }

        public string CreatePath( string fileName = null, string extension = null)
        {
            var now = DateTime.Now;
            var parts = new List<string> { now.Year.ToString()};

            if (Resolution != DateTimePrecision.Year)
                parts.Add(now.Month.ToString());

            if (Resolution != DateTimePrecision.Month)
                parts.Add(now.Day.ToString());

            if (Resolution != DateTimePrecision.Day)
                parts.Add(now.Hour.ToString());

            if (Resolution != DateTimePrecision.Hour)
                parts.Add(now.Minute.ToString());

            if (Resolution != DateTimePrecision.Minute)
                parts.Add(now.Second.ToString());

            if (Resolution != DateTimePrecision.Second)
                parts.Add(now.Millisecond.ToString());

            if (fileName.IsNullOrWhiteSpace())
                fileName = Guid.NewGuid().ToString();

            parts.Add(fileName);

            var path = string.Join(Path.DirectorySeparatorChar.ToString(), parts);

            if (!extension.IsNullOrWhiteSpace())
                path = Path.ChangeExtension(path, extension);

            return path;
        }
    }
}