using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CoreTechs.Common;

namespace BlobServer.Infrastructure
{
    public class DateTimePathCreator : IPathCreator
    {
        public DateTimePathCreator()
        {
            Resolution = DateTimePrecision.Hour;
        }

        public DateTimePrecision Resolution { get; set; }

        public string CreatePath(string rootFolder = null, string fileName = null, string extension = null)
        {
            var now = DateTime.Now;
            var parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(rootFolder))
                parts.Add(rootFolder);

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

        
        public string AppendRandomDirectory(string path)
        {
            var parts = path.SplitPath().ToList();
            var random = (Characters.Keyboard.UpperLetters + Characters.Keyboard.Digits).RandomElement().ToString();
            parts.Insert(parts.Count - 1, random);
            var path2 = string.Join("/", parts);
            return path2;
        }
    }
}