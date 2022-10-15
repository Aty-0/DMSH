using UnityEngine;

namespace DMSH.Objects.Bonuses
{
    public abstract class IBonusBehaviour : ScriptableObject
    {
        [Header("Base Bonus")]
        public Sprite BonusSprite;
        public Color BonusSpriteColor;
        
        public abstract void Use(Bonus caller);

        public virtual void Apply(Bonus caller)
        {
            caller.Renderer.sprite = BonusSprite;
            caller.Renderer.color = BonusSpriteColor;
        }
    }
}