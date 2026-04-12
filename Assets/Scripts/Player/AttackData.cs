public struct AttackData
{
    public float Damage;
    public bool IsShieldPenetrating;

    public AttackData(float damage, bool isShieldPenetrating = false)
    {
        Damage = damage;
        IsShieldPenetrating = isShieldPenetrating;
    }
}