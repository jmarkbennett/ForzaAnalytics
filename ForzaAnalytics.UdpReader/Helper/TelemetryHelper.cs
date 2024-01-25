namespace ForzaAnalytics.UdpReader.Helper
{
    public static class TelemetryHelper
    {
        private static byte[] extractBytes(ref byte[] payload, int startPosition, int endPosition)
        {
            return payload.ToList().GetRange(startPosition, endPosition - startPosition + 1).ToArray();
        }
        public static int extractInt32Value(ref byte[] payload, int startPosition, int endPosition)
        {
            return BitConverter.ToInt32(extractBytes(ref payload, startPosition, endPosition), 0);
        }
        public static uint extractUInt32Value(ref byte[] payload, int startPosition, int endPosition)
        {
            return BitConverter.ToUInt32(extractBytes(ref payload, startPosition, endPosition), 0);
        }
        public static uint extractUInt16Value(ref byte[] payload, int startPosition, int endPosition)
        {
            return BitConverter.ToUInt16(extractBytes(ref payload, startPosition, endPosition), 0);
        }
        public static int extractInt16Value(ref byte[] payload, int startPosition, int endPosition)
        {
            return BitConverter.ToInt16(extractBytes(ref payload, startPosition, endPosition), 0);
        }
        public static float extractSingle(ref byte[] payload, int startPosition, int endPosition)
        {
            return BitConverter.ToSingle(extractBytes(ref payload, startPosition, endPosition), 0);
        }
        public static double extractDouble(ref byte[] payload, int startPosition, int endPosition)
        {
            if (startPosition - endPosition < 7)
            {
                byte[] tmpPayload = new byte[8];
                Array.Copy(extractBytes(ref payload, startPosition, endPosition), tmpPayload, 4);
                return BitConverter.ToDouble(tmpPayload, 0);
            }
            else
                return BitConverter.ToDouble(extractBytes(ref payload, startPosition, endPosition), 0);
        }
    }
}