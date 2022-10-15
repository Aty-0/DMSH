namespace DMSH.Objects.Projectiles
{
    public struct FireStateStruct
    {
        public int StepIndex;
        public float Lifetime;
        public bool IsStarted;
        
        public static FireStateStruct CreateEmpty() => new FireStateStruct
        {
            StepIndex = -1
        };
    }
}