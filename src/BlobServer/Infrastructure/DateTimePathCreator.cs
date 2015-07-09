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
            var parts = new List<string>();

            var dtValues = new[]
            {
                Tuple.Create(DateTimePrecision.Year, now.Year),
                Tuple.Create(DateTimePrecision.Month, now.Month),
                Tuple.Create(DateTimePrecision.Day, now.Day),
                Tuple.Create(DateTimePrecision.Hour, now.Hour),
                Tuple.Create(DateTimePrecision.Minute, now.Minute),
                Tuple.Create(DateTimePrecision.Second, now.Second),
                Tuple.Create(DateTimePrecision.Millisecond, now.Millisecond),
            };

            foreach (var x in dtValues)
            {
                parts.Add(x.Item2.ToString());
                
                if (x.Item1 == Resolution)
                    break;
            }

            if (fileName.IsNullOrWhiteSpace())
                fileName = Guid.NewGuid().ToString();

            parts.Add(fileName);

            var path = string.Join("/", parts);

            if (!extension.IsNullOrWhiteSpace())
                path = Path.ChangeExtension(path, extension);

            return path;
        }
    }
}