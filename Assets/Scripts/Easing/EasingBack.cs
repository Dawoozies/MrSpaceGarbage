using Unity.Mathematics;
public static partial class Easing
{
    public static float easeInOutBack(float x)
    {
        var c1 = 1.70158f;
        var c2 = c1 * 1.525f;

        return x < 0.5f
          ? (math.pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2
          : (math.pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;
    }
}
