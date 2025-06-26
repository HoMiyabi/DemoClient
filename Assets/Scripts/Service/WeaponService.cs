namespace Kirara.Service
{
    public static class WeaponService
    {
        public static void GachaWeapon()
        {
            NetFn.Send(new MsgGachaWeapon());
        }
    }
}