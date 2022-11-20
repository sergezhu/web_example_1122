namespace App.Code.Loader
{
	public class UrlLoader : FirebaseDBLoader<string>
	{
		public UrlLoader( InternetStateService internetStateService ) : base( internetStateService )
		{
		}
	}
}