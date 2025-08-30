using System.Collections.Generic;

namespace Kirara
{
    public class AnimRootMotion
    {
        public float length;
        public int frameRate;
        public List<float> tx = new();
        public List<float> ty = new();
        public List<float> tz = new();
        public List<float> qx = new();
        public List<float> qy = new();
        public List<float> qz = new();
        public List<float> qw = new();
    }
}