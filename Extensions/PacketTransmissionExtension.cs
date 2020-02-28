using Aragas.Network.Packets;

namespace Aragas.Network.IO
{
    public static class PacketTransmissionExtension
    {
        public static bool TryReadPacket<TPacket, TIDType>(this PacketTransmission<TPacket, TIDType> transmission, out TPacket? packet)
            where TPacket : Packet<TIDType>
            => (packet = transmission.ReadPacket()) != null;
    }
}