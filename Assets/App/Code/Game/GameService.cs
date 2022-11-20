namespace App.Code.Game
{
	using UnityEngine;

	public class GameService
	{
		private readonly GameView _view;

		public GameService(GameView view)
		{
			_view = view;
		}

		public void Start()
		{
			Debug.Log( "Game Start" );
		}
	}
}