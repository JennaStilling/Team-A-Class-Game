namespace Observations
{
    public interface IObserver
    {
        public void OnNotify(EnemyAlerts alert);
    }
}