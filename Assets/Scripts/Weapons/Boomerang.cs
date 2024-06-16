namespace Weapons
{
    public class Boomerang : Projectile
    {
        void Update()
        {
            ModifySpeed();

            _myRigidBody.velocity -= _myRigidBody.velocity.normalized * (0.2f * _curSpeed);
        }
    }
}