namespace App.Code.Game
{
	using System;
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
		
		[NonSerialized]
		public string NonSportWords =
			"breakfast,lunch,dinner,bread,milk,water,meat,fish,fruit,vegetables,tea,coffee,juice,cheese,egg,rice,sandwich,apple,banana,cherry,table,chair,bed," +
			"sofa,shelf,armchair,fridge,microwave,cabinet,box,clothes,shoes,suit,blouse,t-shirt,shirt,dress,hat,skirt,glove,scarf,trousers,shorts,jeans,sweater," +
			"jacket,coat,sock,tights,underwear,family,relatives,sister,brother,husband,wife,daughter,son,parents,mother,father,friend,boyfriend,girlfriend,colleague," +
			"neighbor,acquaintance,talk,meet,relationship,visit,guest,chief,boss,music,cinema,park,sing,song,book,film,language,weather,sky,sun,rain,snow," +
			"cloud,temperature,degree,advice,age,air,animal,apparel,apple,badge,birthday,block,branch,bread,brick,building,call,camp,ceiling,day,door," +
			"education,eye,face,fact,gate,glue,hair,heart,holiday,house,king,kitten,money,night,pie,saw,smile,story,thought,tree,wall,water,wood,world,year";

		[NonSerialized]
		public string SportWords =
			"player,sport,race,ball,score,turn,fan,running,exercise,training,athlete,basketball,football,soccer,hockey,chess,league,championship,round,match," +
			"champion,tournament,game,winner,loser,swimming,racing,goal,victory,olympiad,mountaineering,climbing,American football,aerobics,badminton,basketball," +
			"running,jogging,baseball,martial arts,boxing,wrestling,ten-pin bowling,windsurfing,water polo,water skiing,volleyball,handball,gymnastics,golf,motor " +
			"racing,rowing,canoeing,judo,darts,cycling,bowls,yoga,karate,karting,inline skating,kick boxing,horse riding,cricket,lacrosse,athletics,skiing," +
			"tennis,netball,hunting,hiking,swimming,sailing,beach volleyball,scuba diving,weightlifting,diving,pool,rugby,fishing,surfing,horse racing,squash,skateboarding," +
			"snowboarding,snooker,shooting,archery,ice skating,football,walking,hockey,ice hockey";
		
		public int WordsGroupSize = 4;
	}
}