namespace CoreCI.Common.Infrastructure.Extensions
{
    public static class IntegerExtensions
    {
        public static int EnsureAtLeast( this int operand, int minimum ) => operand < minimum ? minimum : operand;
    }
}
