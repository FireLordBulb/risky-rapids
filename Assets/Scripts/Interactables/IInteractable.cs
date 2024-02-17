using UnityEngine;
public interface IInteractable
{
    public void OnCollisionDetected();
    public void OnCollisionDetected(Player player);
    public void OnCollisionDetected(Player player, Vector3 collisionPoint);
    public void Activate();
    public void Deactivate();

    public void Register();
}
