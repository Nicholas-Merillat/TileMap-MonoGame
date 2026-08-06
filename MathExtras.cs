namespace TileMap
{
    internal static class MathExtras
    {   
        // Why is this not included in MonoGame by default???
        public static double Remap(double value, double low1, double high1, double low2, double high2)
        {
            return low2 + (value - low1) * (high2 - low2) / (high1 - low1);
        }
    }
}
