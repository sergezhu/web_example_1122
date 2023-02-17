namespace App.Code.Utils
{
	using UnityEngine;

	public static class MathCore
	{
		public static float Epsilon => float.Epsilon;
		public static float Abs( float v ) => Mathf.Abs( v );
		public static int Abs( int v ) => Mathf.Abs( v );
		public static float Sign( float v ) => Abs( v ) < Epsilon ? 0 : v > 0 ? 1f : -1f;
		public static int Sign( int v ) => v == 0 ? 0 : v > 0 ? 1 : -1;


		public static double Clamp01( double value )
		{
			return value < 0.0
					? 0.0
					: value > 1.0
						? 1.0
						: value;
		}

		public static double Clamp( double value, double min, double max )
		{
			// https://github.com/Unity-Technologies/UnityCsReference/blob/master/Runtime/Export/Math/Mathf.cs

			return value < min
					? min
					: value > max
						? max
						: value;
		}
	}
}