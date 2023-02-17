namespace App.Code.Game
{
	using System;
	using System.Linq;
	using App.Code.Utils;
	using UnityEngine;
	using UnityEngine.UI;

	public class SpriteRandomizer : MonoBehaviour
	{
		[SerializeField] private Image[] _targetImages;
		[SerializeField] private Sprite[] _sprites;
		
		public int SpriteIndex { get; private set; }


		public void Shuffle(int[] excludeIndexes = null)
		{
			if ( excludeIndexes == null )
			{
				SpriteIndex = _sprites.RandomIndex();
			}
			else
			{
				if ( excludeIndexes.Length >= _sprites.Length )
					throw new InvalidOperationException();

				do
				{
					SpriteIndex = _sprites.RandomIndex();
					
				} while ( excludeIndexes.Count( i => SpriteIndex == i ) != 0 );
			}
			
			var randomFlag = _sprites[SpriteIndex];
			
			_targetImages.ForEach( img => img.sprite = randomFlag );
		}
	}
}