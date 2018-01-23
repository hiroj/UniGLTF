﻿using System;
using System.IO;
using System.Text;

namespace UniGLTF
{
    public enum GlbChunkType : UInt32
    {
        JSON = 0x4E4F534A,
        BIN = 0x004E4942,
    }

    public struct GlbHeader
    {
        public static void WriteTo(Stream s)
        {
            s.WriteByte((Byte)'g');
            s.WriteByte((Byte)'l');
            s.WriteByte((Byte)'T');
            s.WriteByte((Byte)'F');
            var bytes = BitConverter.GetBytes((UInt32)2);
            s.Write(bytes, 0, bytes.Length);
        }
    }

    public struct GlbChunk
    {
        public GlbChunkType ChunkType;
        public ArraySegment<Byte> Bytes;

        public GlbChunk(string json) : this(
            GlbChunkType.JSON,
            new ArraySegment<byte>(Encoding.UTF8.GetBytes(json))
            )
        {
        }

        public GlbChunk(ArraySegment<Byte> bytes) : this(
            GlbChunkType.BIN,
            bytes
            )
        {
        }

        public GlbChunk(GlbChunkType type, ArraySegment<Byte> bytes)
        {
            ChunkType = type;
            Bytes = bytes;
        }

        public int WriteTo(Stream s)
        {
            // size
            var bytes = BitConverter.GetBytes((int)Bytes.Count);
            s.Write(bytes, 0, bytes.Length);

            // chunk type
            switch (ChunkType)
            {
                case GlbChunkType.JSON:
                    s.WriteByte((byte)'J');
                    s.WriteByte((byte)'S');
                    s.WriteByte((byte)'O');
                    s.WriteByte((byte)'N');
                    break;

                case GlbChunkType.BIN:
                    s.WriteByte((byte)' ');
                    s.WriteByte((byte)'B');
                    s.WriteByte((byte)'I');
                    s.WriteByte((byte)'N');
                    break;

                default:
                    throw new Exception("unknown chunk type: " + ChunkType);
            }

            // body
            s.Write(Bytes.Array, Bytes.Offset, Bytes.Count);

            return 4 + 4 + Bytes.Count;
        }
    }
}
