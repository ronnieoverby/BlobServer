﻿using System;
using System.IO;

namespace BlobServer.Infrastructure
{
    public static class Extensions
    {
        private static readonly char[] DirSeparators = { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };

        public static string[] SplitPath(this string path)
        {
            if (path == null) throw new ArgumentNullException("path");
            var parts = path.Trim(DirSeparators).Split(DirSeparators);
            return parts;
        }
    }
}