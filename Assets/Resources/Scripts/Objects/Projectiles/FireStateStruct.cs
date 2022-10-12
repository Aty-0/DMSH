namespace DMSH.Objects.Projectiles
{
    public struct FireStateStruct
    {
        public int StepIndex;
        public float Lifetime;
        
        public static FireStateStruct CreateEmpty() => new FireStateStruct
        {
            StepIndex = -1
        };
    }
}