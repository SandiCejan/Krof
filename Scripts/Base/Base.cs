namespace KrofEngine
{
    public enum TextAlignment
    {
        Left, Middle, Right
    }
    public class Base
    {
        public static float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}
