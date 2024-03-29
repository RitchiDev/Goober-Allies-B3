using UnityEngine;

public static class MyExtensions
{
	public static Vector2Int GetPositionToVector2Int(Vector3 position)
	{
		Vector2Int intPosition = new Vector2Int((int)position.x, (int)position.z);
		return intPosition;
	}

	/// <summary>
	/// Returns a random color.
	/// </summary>
	/// <returns></returns>
	public static Color GetRandomColor()
    {
        return new Color(Random.value, Random.value, Random.value);
    }

	public static int IntLoopClamp(int value, int minValue, int maxValue)
	{
		while (value < minValue || value >= maxValue)
		{
			if (value < minValue)
			{
				value += maxValue - minValue;
			}
			else if (value >= maxValue)
			{
				value -= maxValue - minValue;
			}
		}

		return value;
	}

	public static float FloatLoopClamp(float value, float minValue, float maxValue)
	{
		while (value < minValue || value >= maxValue)
		{
			if (value < minValue)
			{
				value += maxValue - minValue;
			}
			else if (value >= maxValue)
			{
				value -= maxValue - minValue;
			}
		}

		return value;
	}

	/// <summary>
	/// Creates a copy of a vector while replacing the desired x and/or y value(s).
	/// Usage example: Vector2 groundedPosition = somePosition.With(y: 0f);
	/// </summary>
	public static Vector2 With(this Vector2 vector, float? x = null, float? y = null)
	{
		return new Vector2
		(
			x ?? vector.x,
			y ?? vector.y
		);
	}

	/// <summary>
	/// Creates a copy of a vector while replacing the desired x, y and/or z value(s).
	/// Usage example: Vector3 groundedPosition = somePosition.With(y: 0f);
	/// </summary>
	public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null)
	{
		return new Vector3
		(
			x ?? vector.x,
			y ?? vector.y,
			z ?? vector.z
		);
	}
}
