using System;
using System.Timers;
namespace GPRO.Core.Generic
{
	public class ServiceManager<T>
	{
		private const int TRYCONNECTCOUNT = 10;
		private Func<T> _funcIns;
		private Action<T> _actionPing;
		private T _svc;
		private Timer _timer;
		private int _tryConnect = 0;
		private void Ping()
		{
			try
			{
				if (this._actionPing != null)
				{
					this._actionPing(this._svc);
				}
			}
			catch (Exception ex)
			{
				if (this._tryConnect >= 10)
				{
					this._tryConnect = 0;
					throw ex;
				}
				this._tryConnect++;
				this._svc = this._funcIns();
				this.Ping();
			}
		}
		public ServiceManager(Func<T> funcIns, Action<T> actionPing)
		{
			this._funcIns = funcIns;
			this._actionPing = actionPing;
			this._svc = this._funcIns();
			if (this._actionPing != null)
			{
				this._timer = new Timer(240000.0);
				this._timer.AutoReset = true;
				this._timer.Elapsed += new ElapsedEventHandler(this._timer_Elapsed);
			}
		}
		private void _timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			this.Ping();
		}
		public T GetService()
		{
			this.Ping();
			return this._svc;
		}
	}
}
