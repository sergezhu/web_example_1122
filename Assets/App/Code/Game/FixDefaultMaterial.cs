namespace Game.Code.UI
{
	using UnityEngine;
	using UnityEngine.UI;

	[RequireComponent(typeof(Image))]
	public class FixDefaultMaterial : MonoBehaviour
	{
		[ContextMenu("Fix")]
		private void Fix()
		{
			GetComponent<Image>().material.color = new Color( 1, 1, 1, 1 );
		}
	}
}