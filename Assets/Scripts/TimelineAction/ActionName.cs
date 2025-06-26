namespace Kirara
{
    public static class ActionName
    {
        // Locomotion
        public const string Evade_Back = "Evade_Back";
        public const string Evade_Front = "Evade_Front";
        public const string Idle = "Idle";
        public const string Run_End = "Run_End";
        public const string Run = "Run";
        public const string SwitchIn_Normal = "SwitchIn_Normal";
        public const string TurnBack = "TurnBack";
        public const string Walk_Start = "Walk_Start";
        public const string Walk = "Walk";
        public const string SwitchOut_Normal = "SwitchOut_Normal";
        public const string Background = "Background";

        // Attack
        public const string Attack_ParryAid_Start = "Attack_ParryAid_Start";
        public const string Attack_ParryAid_H = "Attack_ParryAid_H";
        public const string Attack_ParryAid_H_End = "Attack_ParryAid_H_End";
        public const string Attack_ParryAid_L = "Attack_ParryAid_L";
        public const string Attack_ParryAid_L_End = "Attack_ParryAid_L_End";

        public const string Attack_Rush = "Attack_Rush";
        public const string Attack_Rush_End = "Attack_Rush_End";
        public const string SwitchIn_Attack_Ex_Start = "SwitchIn_Attack_Ex_Start";
        public const string SwitchIn_Attack_Ex = "SwitchIn_Attack_Ex";
        public const string Attack_Special = "Attack_Special";
        public const string Attack_Ex_Special = "Attack_Ex_Special";

        public const string Attack_Normal_01 = "Attack_Normal_01";
        public const string Attack_Normal_02 = "Attack_Normal_02";
        public const string Attack_Normal_03 = "Attack_Normal_03";
        public const string Attack_Normal_04 = "Attack_Normal_04";
        public const string Attack_Normal_05 = "Attack_Normal_05";
        public const string Attack_Normal_06 = "Attack_Normal_06";

        public const string Attack_01 = "Attack_01";
        public const string Attack_02 = "Attack_02";

        public static string Attack_Normal(int segment)
        {
            return segment switch
            {
                1 => Attack_Normal_01,
                2 => Attack_Normal_02,
                3 => Attack_Normal_03,
                4 => Attack_Normal_04,
                5 => Attack_Normal_05,
                6 => Attack_Normal_06,
                _ => $"Attack_Normal_{segment:D2}"
            };
        }

        public const string Hit_L_Front = "Hit_L_Front";
        public const string Hit_L_Back = "Hit_L_Back";
    }
}