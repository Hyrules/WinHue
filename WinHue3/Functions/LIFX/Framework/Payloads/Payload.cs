namespace WinHue3.LIFX.Framework.Payloads
{
    public abstract class Payload
    {
        public abstract int Length { get; }
        public abstract byte[] GetBytes();

        public static implicit operator byte[](Payload payload)
        {
            return payload.GetBytes();
        }
    }
}
