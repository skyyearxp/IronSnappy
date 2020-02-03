using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IronSnappy
{
   class SnappyWriter : Stream
   {
      private const string MagicBody = "sNaPpY";
      private const string MagicChunk = "\xff\x06\x00\x00" + MagicBody;
      private const int MaxBlockSize = 65536;
      private const int ChecksumSize = 4;
      private const int ChunkHeaderSize = 4;
      private const int MaxEncodedLenOfMaxBlockSize = 76490;

      private static readonly int ObufHeaderLen = MagicChunk.Length + ChecksumSize + ChunkHeaderSize;
      private static readonly int ObufLen = ObufHeaderLen + MaxEncodedLenOfMaxBlockSize;

      private readonly Stream _parent;
      private static ArrayPool<byte> BytePool = ArrayPool<byte>.Shared;

      private byte[] _ibuf;
      private byte[] _obuf;
      private int _ibufIdx;
      private bool _wroteStreamHeader;

      public SnappyWriter(Stream parent)
      {
         _parent = parent;

         _ibuf = BytePool.Rent(MaxBlockSize);
         _obuf = BytePool.Rent(ObufLen);
      }

      public override bool CanRead => false;

      public override bool CanSeek => false;

      public override bool CanWrite => true;

      public override long Length => _parent.Length;

      public override long Position { get => _parent.Position; set => throw new NotSupportedException(); }

      public override void Flush()
      {
         if(_ibufIdx == 0)
            return;

         WriteChunk(_ibuf.AsSpan(.._ibufIdx));
         _ibufIdx = 0;
      }

      public override int Read(byte[] buffer, int offset, int count) => throw new NotImplementedException();
      public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();
      public override void SetLength(long value) => throw new NotImplementedException();

      public override void Write(byte[] buffer, int offset, int count)
      {
         Span<byte> src = buffer.AsSpan(offset, count);

         while(src.Length > (_ibuf.Length - _ibufIdx))
         {
            int copyMax;

            if(_ibufIdx == 0)
            {
               // large write, empty buffer, can write directly from src
               WriteChunk(src);
               copyMax = src.Length;
            }
            else
            {
               //append to ibuf what we can
               copyMax = _ibuf.Length - _ibufIdx;
               src[..copyMax].CopyTo(_ibuf.AsSpan(_ibufIdx..));
               _ibufIdx += copyMax;

               Flush();
            }

            src = src[copyMax..];
         }

         //copy remaining data
         int copyMaxLeft = Math.Min(_ibuf.Length - _ibufIdx, src.Length);
         src[..copyMaxLeft].CopyTo(_ibuf.AsSpan(_ibufIdx..));
         _ibufIdx += copyMaxLeft;
      }

      protected override void Dispose(bool disposing)
      {
         try
         {
            Flush();
         }
         finally
         {
            BytePool.Return(_ibuf);
            BytePool.Return(_obuf);
         }
      }

      private void WriteChunk(ReadOnlySpan<byte> p)
      {
         while(p.Length > 0)
         {
            int obufStart = MagicChunk.Length;

            if(!_wroteStreamHeader)
            {
               _wroteStreamHeader = true;
               Encoding.UTF8.GetBytes(MagicChunk).AsSpan().CopyTo(_obuf.AsSpan());
               obufStart = 0;
            }

            ReadOnlySpan<byte> uncompressed;
            if(p.Length > MaxBlockSize)
            {
               uncompressed = p[..MaxBlockSize];
               p = p[MaxBlockSize..];
            }
            else
            {
               uncompressed = p;
               p = null;
            }

            uint checksum = Crc32.Compute(uncompressed);

            // Compress the buffer, discarding the result if the improvement
            // isn't at least 12.5%.
            ReadOnlySpan<byte> compressed = Encode(_obuf.AsSpan()[ObufHeaderLen..], uncompressed);
         }
      }

      ReadOnlySpan<byte> Encode(Span<byte> dst, ReadOnlySpan<byte> src)
      {
         int n = GetMaxEncodedLen(src.Length);
         if(n < 0)
         {
            throw new ArgumentException("block is too large", nameof(src));
         }
         else if(dst.Length < n)
         {
            throw new NotImplementedException();
         }

         throw new NotImplementedException();
      }

      private static int GetMaxEncodedLen(int srcLen)
      {
         uint n = (uint)srcLen;
	      if(n > 0xffffffff)
         {
            return -1;
	      }
         // Compressed data can be defined as:
         //    compressed := item* literal*
         //    item       := literal* copy
         //
         // The trailing literal sequence has a space blowup of at most 62/60
         // since a literal of length 60 needs one tag byte + one extra byte
         // for length information.
         //
         // Item blowup is trickier to measure. Suppose the "copy" op copies
         // 4 bytes of data. Because of a special check in the encoding code,
         // we produce a 4-byte copy only if the offset is < 65536. Therefore
         // the copy op takes 3 bytes to encode, and this type of item leads
         // to at most the 62/60 blowup for representing literals.
         //
         // Suppose the "copy" op copies 5 bytes of data. If the offset is big
         // enough, it will take 5 bytes to encode the copy op. Therefore the
         // worst case here is a one-byte literal followed by a five-byte copy.
         // That is, 6 bytes of input turn into 7 bytes of "compressed" data.
         //
         // This last factor dominates the blowup, so the final estimate is:
         n = 32 + n + n / 6;
	      if(n > 0xffffffff)
         {
            return -1;
	      }
         return (int)n;
      }
   }
}
