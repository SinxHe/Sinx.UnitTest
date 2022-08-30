using System.Numerics;

namespace Sinx.LinearAlgebra.Shared;

public static class VectorExtensions
{
	public static Vector3 ToVector3(this Vector2 v)
	{
		return new(v.X, v.Y, 0);
	}
}
