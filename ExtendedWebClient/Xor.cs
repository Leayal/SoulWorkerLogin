using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Leayal
{
    public static class Xor
    {
        public const System.Int32 SecretByte = 0x55;
        public static byte[] XorByteArray(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] ^= Xor.SecretByte;
            return buffer;
        }
        public static void XorFile(string srcFile)
        {
            using (var memStream = new MemoryStream())
            using (var fs = File.Open(srcFile, FileMode.Open))
            {
                XorFile(fs, memStream);
                StreamCopy(memStream, fs);
            }
        }
        public static void XorFile(string srcFile, string outFile)
        {
            using (var read = File.OpenRead(srcFile))
            using (var write = File.OpenWrite(outFile))
            { XorFile(read, write); }
        }
        public static void XorFile(Stream src, Stream output)
        {
            int bytesread;
            byte[] buffer = new byte[1024];
            bytesread = src.Read(buffer, 0, 1024);
            while (bytesread > 0)
            {
                output.Write(XorByteArray(buffer), 0, bytesread);
                bytesread = src.Read(buffer, 0, 1024);
            }
        }
        public static void StreamCopySegment(Stream src, Stream output)
        {
            int bytesread;
            byte[] buffer = new byte[1024];
            bytesread = src.Read(buffer, 0, 1024);
            while (bytesread > 0)
            {
                output.Write(buffer, 0, bytesread);
                bytesread = src.Read(buffer, 0, 1024);
            }
        }
        public static void StreamCopy(Stream src, Stream output)
        {
            src.Position = 0;
            output.Position = 0;
            output.SetLength(src.Length);
            int bytesread;
            byte[] buffer = new byte[1024];
            bytesread = src.Read(buffer, 0, 1024);
            while (bytesread > 0)
            {
                output.Write(buffer, 0, bytesread);
                bytesread = src.Read(buffer, 0, 1024);
            }
        }

        public class XorStream : System.IO.FileStream
        {
            public XorStream(string Path) : base(Path, FileMode.OpenOrCreate)
            {
            }

            public XorStream(string Path, FileMode fileMode, FileAccess fileAccess) : base(Path, fileMode, fileAccess)
            {
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                int theRead = base.Read(buffer, offset, count);
                for (int i = 0; i < buffer.Length; i++)
                    buffer[i] ^= Xor.SecretByte;
                return theRead;
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                for (int i = 0; i < buffer.Length; i++)
                    buffer[i] ^= Xor.SecretByte;
                base.Write(buffer, offset, count);
            }

            public override void WriteByte(byte value)
            {
                value ^= Xor.SecretByte;
                base.WriteByte(value);
            }

            public override int ReadByte()
            {
                return (base.ReadByte() ^ Xor.SecretByte);
            }
        }
    }    
}
