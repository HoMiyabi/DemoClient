using System.Collections.Generic;

namespace Kirara
{
    public class AnimRootMotion
    {
        public List<AnimKeyframe> tx = new();
        public List<AnimKeyframe> ty = new();
        public List<AnimKeyframe> tz = new();
        public List<AnimKeyframe> qx = new();
        public List<AnimKeyframe> qy = new();
        public List<AnimKeyframe> qz = new();
        public List<AnimKeyframe> qw = new();
    }
}