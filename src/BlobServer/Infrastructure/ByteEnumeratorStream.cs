using System;
using System.Collections.Generic;
using System.IO;

namespace BlobServer.Infrastructure
{
    public class ByteEnumeratorStream : Stream
    {
        private readonly IEnumerator<byte> _it;
        private readonly IEnumerator<byte> _ownedEnumerator;

        public ByteEnumeratorStream(IEnumerator<byte> enumerator)
        {
            if (enumerator == null) throw new ArgumentNullException("bytes");
            _it = enumerator;
        }

        public ByteEnumeratorStream(IEnumerable<byte> bytes)
        {
            if (bytes == null) throw new ArgumentNullException("bytes");
            _it = _ownedEnumerator = bytes.GetEnumerator();
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var read = 0;
            for (var i = 0; i < count; i++)
            {
                if (!_it.MoveNext())
                    break;

                buffer[i + offset] = _it.Current;

                read++;
            }

            return read;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        protected override void Dispose(bool disposing)
        {
            using (_ownedEnumerator)
                base.Dispose(disposing);
        }
    }
}