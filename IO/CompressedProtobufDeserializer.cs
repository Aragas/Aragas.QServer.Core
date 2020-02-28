using Aragas.Network.Data;
using Aragas.Network.IO;

using Ionic.Zlib;

using System;
using System.IO;

namespace Aragas.QServer.Core.IO
{
    public class CompressedProtobufDeserializer : ProtobufDeserializer
    {
        public int CompressionThreshold { get; set; } = 256;

        public CompressedProtobufDeserializer() { }
        public CompressedProtobufDeserializer(in Span<byte> data) : base(in data) { }
        public CompressedProtobufDeserializer(Stream stream) : base(stream) { }

        protected override void Initialize(Stream stream)
        {
            if(CompressionThreshold == -1)
                Initialize(stream);
            Stream = stream;

            // N | Packet Length | Length of Data Length + compressed length of (Packet ID + Data)
            // N | Data Length   | Length of uncompressed (Packet ID + Data) or 0
            // C | Data          | zlib compressed packet data (see the sections below)

            var packetLength = Read<VarInt>();
            var dataLength = Read<VarInt>();
            var actualDataLength = packetLength - new VarInt(dataLength).Size;
            var actualData = Read(Array.Empty<byte>(), actualDataLength);

            var data = dataLength == 0 ? actualData : ZlibStream.UncompressBuffer(actualData);
            Stream = new MemoryStream(data);
        }
    }
}