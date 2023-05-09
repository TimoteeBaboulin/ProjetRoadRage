using Player;

namespace PowerUps{
	public class SuspensionPickUp : PickUp{
		public override void Activate(PlayerCar player){
			Destroy(gameObject);
			player.GogoGadgetSuspension(_powerUp.Time);
			player.UsePowerUp(_powerUp);
		}
	}
}