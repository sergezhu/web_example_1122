namespace App.Code.Game
{
	using UnityEngine;

	public class GameSettings : MonoBehaviour
	{
		[Header( "Buttons" )]
		[SerializeField] public Sprite DefaultState;
		[SerializeField] public Sprite RightState;
		[SerializeField] public Sprite FalseState;
		[SerializeField] public Sprite InactiveState;

		[Space]
		[SerializeField] public Sprite NextDefaultState;
		[SerializeField] public Sprite NextInactiveState;
		
		[HideInInspector]
		public string NonSportWords =
			"breakfast,lunch,dinner,bread,milk,water,meat,fish,fruit,vegetables,tea,coffee,juice,cheese,egg,rice,sandwich,apple,banana,cherry,table,chair,bed," +
			"sofa,shelf,armchair,fridge,microwave,cabinet,box,clothes,shoes,suit,blouse,t-shirt,shirt,dress,hat,skirt,glove,scarf,trousers,shorts,jeans,sweater," +
			"jacket,coat,sock,tights,underwear,family,relatives,sister,brother,husband,wife,daughter,son,parents,mother,father,daddy,mummy,Grandmother,Grandfather," +
			"uncle,aunt,nephew,niece,cousin,person,people,man,woman,men,women,child,children,boy,girl,friend,boyfriend,girlfriend,colleague,neighbor,acquaintance," +
			"partner,talk,meet,relationship,visit,guest,chief,boss,relax,music,cinema,park,swimming,sport,play,sing,song,book,film,language,weather,sky,sun,rain,snow," +
			"cloud,temperature,degree";

		[HideInInspector]
		public string SportWords =
			"player,sport,race,ball,score,turn,fan,run (running),exercise,training,athlete,basketball,football,soccer,hockey,chess,league,championship,round,match," +
			"champion,tournament,game,winner,loser,swimming";
		
		public int WordsGroupSize = 4;
	}
}