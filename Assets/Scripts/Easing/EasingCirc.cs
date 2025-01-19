using Unity.Mathematics;
public static partial class Easing
{
    public static float easeOutCirc(float x)
    {
        return math.sqrt(1 - math.pow(x - 1, 2));
    }
}