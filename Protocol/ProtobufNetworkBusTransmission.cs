using Aragas.Network.Data;
using Aragas.Network.IO;
using Aragas.Network.Packets;
using Aragas.QServer.NetworkBus;

using System;

namespace Aragas.QServer.Core.Protocol
{
    public abstract class ProtobufNetworkBusTransmission<TProtobufPacketType> : SocketPacketNetworkBusTransmission<TProtobufPacketType, VarInt, ProtobufSerializer, ProtobufDeserializer>
        where TProtobufPacketType : Packet<VarInt>
    {
        protected ProtobufNetworkBusTransmission(IAsyncNetworkBus networkBus, Guid playerId, BasePacketFactory<TProtobufPacketType, VarInt>? factory = null) : base(networkBus, playerId, factory) { }
    }
}