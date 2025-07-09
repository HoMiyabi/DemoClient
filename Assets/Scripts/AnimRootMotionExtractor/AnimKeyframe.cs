namespace Kirara
{
    public struct AnimKeyframe
    {
        public float time;
        public float value;

        public AnimKeyframe(float time, float value)
        {
            this.time = time;
            this.value = value;
        }
    }
}