namespace Infrastructure.Extensions
{
    public static class NumberExtensions
    {
        public static int GetClosestNumberToPowerOf16(int input)
        {

            input |= input >> 1;
            input |= input >> 2;
            input |= input >> 4;
            input |= input >> 8;
            input |= input >> 16;

            int nextPowerOfTwo = input + 1;

            return nextPowerOfTwo < 16 ? 16 : nextPowerOfTwo;
        }
    }
}
