namespace DMSH.Scripts.Objects.Projectiles
{
    public struct ProjectileModificatorStateStruct
    {
        public ProjectileModificatorEnum Type;

        public WallsFlags AffectedWalls;
        public int BounceCount;

        public ProjectileModificatorStateStruct(ProjectileModificator modificatorSetting)
        {
            Type = modificatorSetting.Type;

            switch (Type)
            {
                case ProjectileModificatorEnum.BounceFromWalls:
                    AffectedWalls = modificatorSetting.AffectedWalls;
                    BounceCount = modificatorSetting.BounceCount;
                    break;

                case ProjectileModificatorEnum.Unset:
                    AffectedWalls = default;
                    BounceCount = default;
                    break;

                default:
                    throw new System.NotImplementedException($"Type {Type} not implemented! fix it now!");
            }
        }
    }
}