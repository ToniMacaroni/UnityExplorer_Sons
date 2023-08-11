using UnityEngine;

namespace Vectrosity
{
	public class VectorChar
	{
		public const int numberOfCharacters = 256;

		private static Vector2[][] points;

		public static Vector2[][] data
		{
			get
			{
				if (points == null)
				{
					points = new Vector2[256][];
					points[33] = new Vector2[4]
					{
						new Vector2(0f, -0.9f),
						new Vector2(0f, -1f),
						new Vector2(0f, 0f),
						new Vector2(0f, -0.75f)
					};
					points[34] = new Vector2[4]
					{
						new Vector2(0.15f, 0f),
						new Vector2(0.15f, -0.25f),
						new Vector2(0.45f, -0.25f),
						new Vector2(0.45f, 0f)
					};
					points[35] = new Vector2[8]
					{
						new Vector2(0.2f, 0f),
						new Vector2(0.2f, -1f),
						new Vector2(0f, -0.33f),
						new Vector2(0.6f, -0.33f),
						new Vector2(0.4f, 0f),
						new Vector2(0.4f, -1f),
						new Vector2(0f, -0.66f),
						new Vector2(0.6f, -0.66f)
					};
					points[37] = new Vector2[18]
					{
						new Vector2(0f, 0f),
						new Vector2(0f, -0.25f),
						new Vector2(0.15f, 0f),
						new Vector2(0.15f, -0.25f),
						new Vector2(0f, -0.25f),
						new Vector2(0.15f, -0.25f),
						new Vector2(0f, 0f),
						new Vector2(0.15f, 0f),
						new Vector2(0.6f, -0.75f),
						new Vector2(0.45f, -0.75f),
						new Vector2(0.6f, -1f),
						new Vector2(0.45f, -1f),
						new Vector2(0.45f, -1f),
						new Vector2(0.45f, -0.75f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, -0.75f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, 0f)
					};
					points[38] = new Vector2[16]
					{
						new Vector2(0.2f, -0.5f),
						new Vector2(0.2f, 0f),
						new Vector2(0f, -0.5f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f),
						new Vector2(0.2f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, -0.7f),
						new Vector2(0.2f, 0f),
						new Vector2(0.5f, 0f),
						new Vector2(0.5f, -0.5f),
						new Vector2(0.5f, 0f),
						new Vector2(0f, -0.5f),
						new Vector2(0.5f, -0.5f)
					};
					points[39] = new Vector2[2]
					{
						new Vector2(0.3f, -0.25f),
						new Vector2(0.45f, 0f)
					};
					points[40] = new Vector2[6]
					{
						new Vector2(0.45f, 0f),
						new Vector2(0.15f, -0.25f),
						new Vector2(0.15f, -0.25f),
						new Vector2(0.15f, -0.75f),
						new Vector2(0.45f, -1f),
						new Vector2(0.15f, -0.75f)
					};
					points[41] = new Vector2[6]
					{
						new Vector2(0.15f, 0f),
						new Vector2(0.45f, -0.25f),
						new Vector2(0.45f, -0.25f),
						new Vector2(0.45f, -0.75f),
						new Vector2(0.15f, -1f),
						new Vector2(0.45f, -0.75f)
					};
					points[42] = new Vector2[8]
					{
						new Vector2(0.3f, -1f),
						new Vector2(0.3f, 0f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0.5f, -0.1f),
						new Vector2(0.1f, -0.9f),
						new Vector2(0.5f, -0.9f),
						new Vector2(0.1f, -0.1f)
					};
					points[43] = new Vector2[4]
					{
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0.3f, -0.9f),
						new Vector2(0.3f, -0.1f)
					};
					points[44] = new Vector2[2]
					{
						new Vector2(0f, -1f),
						new Vector2(0.15f, -0.75f)
					};
					points[45] = new Vector2[2]
					{
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.5f)
					};
					points[46] = new Vector2[2]
					{
						new Vector2(0f, -0.9f),
						new Vector2(0f, -1f)
					};
					points[47] = new Vector2[2]
					{
						new Vector2(0.6f, 0f),
						new Vector2(0f, -1f)
					};
					points[48] = new Vector2[8]
					{
						new Vector2(0f, -1f),
						new Vector2(0f, 0f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, 0f),
						new Vector2(0.6f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f)
					};
					points[49] = new Vector2[2]
					{
						new Vector2(0.3f, -1f),
						new Vector2(0.3f, 0f)
					};
					points[50] = new Vector2[10]
					{
						new Vector2(0.6f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0.6f, 0f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f)
					};
					points[51] = new Vector2[8]
					{
						new Vector2(0.6f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.5f)
					};
					points[52] = new Vector2[6]
					{
						new Vector2(0f, -0.5f),
						new Vector2(0f, 0f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, 0f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f)
					};
					points[53] = new Vector2[10]
					{
						new Vector2(0f, 0f),
						new Vector2(0.6f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f)
					};
					points[54] = new Vector2[10]
					{
						new Vector2(0f, 0f),
						new Vector2(0.6f, 0f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f),
						new Vector2(0f, 0f),
						new Vector2(0f, -1f)
					};
					points[55] = new Vector2[4]
					{
						new Vector2(0f, 0f),
						new Vector2(0.6f, 0f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, 0f)
					};
					points[56] = new Vector2[10]
					{
						new Vector2(0f, 0f),
						new Vector2(0.6f, 0f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f)
					};
					points[57] = new Vector2[10]
					{
						new Vector2(0f, 0f),
						new Vector2(0.6f, 0f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, 0f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0f, 0f),
						new Vector2(0f, -0.5f)
					};
					points[58] = new Vector2[4]
					{
						new Vector2(0f, -0.9f),
						new Vector2(0f, -1f),
						new Vector2(0f, -0.3f),
						new Vector2(0f, -0.4f)
					};
					points[59] = new Vector2[4]
					{
						new Vector2(0f, -1f),
						new Vector2(0.15f, -0.75f),
						new Vector2(0.1f, -0.3f),
						new Vector2(0.1f, -0.4f)
					};
					points[60] = new Vector2[4]
					{
						new Vector2(0.6f, 0f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -0.5f)
					};
					points[61] = new Vector2[4]
					{
						new Vector2(0.6f, -0.25f),
						new Vector2(0f, -0.25f),
						new Vector2(0.6f, -0.75f),
						new Vector2(0f, -0.75f)
					};
					points[62] = new Vector2[4]
					{
						new Vector2(0f, 0f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -0.5f)
					};
					points[63] = new Vector2[10]
					{
						new Vector2(0f, -0.9f),
						new Vector2(0f, -1f),
						new Vector2(0f, -0.75f),
						new Vector2(0f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0.3f, -0.5f),
						new Vector2(0.3f, 0f),
						new Vector2(0.3f, -0.5f),
						new Vector2(0f, 0f),
						new Vector2(0.3f, 0f)
					};
					points[65] = new Vector2[10]
					{
						new Vector2(0f, -1f),
						new Vector2(0f, -0.3f),
						new Vector2(0.6f, -0.3f),
						new Vector2(0.6f, -1f),
						new Vector2(0.3f, 0f),
						new Vector2(0f, -0.3f),
						new Vector2(0.3f, 0f),
						new Vector2(0.6f, -0.3f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.5f)
					};
					points[66] = new Vector2[20]
					{
						new Vector2(0f, -1f),
						new Vector2(0f, 0f),
						new Vector2(0.447f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0.447f, 0f),
						new Vector2(0.6f, -0.155f),
						new Vector2(0.6f, -0.347f),
						new Vector2(0.6f, -0.155f),
						new Vector2(0.448f, -0.5f),
						new Vector2(0.6f, -0.347f),
						new Vector2(0.448f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.653f),
						new Vector2(0.448f, -0.5f),
						new Vector2(0.6f, -0.653f),
						new Vector2(0.6f, -0.845f),
						new Vector2(0.447f, -1f),
						new Vector2(0.6f, -0.845f),
						new Vector2(0f, -1f),
						new Vector2(0.447f, -1f)
					};
					points[67] = new Vector2[6]
					{
						new Vector2(0.6f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f)
					};
					points[68] = new Vector2[12]
					{
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0.447f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0.447f, 0f),
						new Vector2(0.6f, -0.155f),
						new Vector2(0.6f, -0.845f),
						new Vector2(0.6f, -0.155f),
						new Vector2(0.6f, -0.845f),
						new Vector2(0.447f, -1f),
						new Vector2(0.447f, -1f),
						new Vector2(0f, -1f)
					};
					points[69] = new Vector2[8]
					{
						new Vector2(0f, 0f),
						new Vector2(0.6f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0.3f, -0.5f)
					};
					points[70] = new Vector2[6]
					{
						new Vector2(0f, 0f),
						new Vector2(0.6f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0.3f, -0.5f)
					};
					points[71] = new Vector2[10]
					{
						new Vector2(0.6f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0f, 0f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f),
						new Vector2(0.3f, -0.5f),
						new Vector2(0.6f, -0.5f)
					};
					points[72] = new Vector2[6]
					{
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, 0f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.5f)
					};
					points[73] = new Vector2[6]
					{
						new Vector2(0.6f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f),
						new Vector2(0.3f, -1f),
						new Vector2(0.3f, 0f)
					};
					points[74] = new Vector2[6]
					{
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0f, -0.725f)
					};
					points[75] = new Vector2[6]
					{
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, 0f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -1f)
					};
					points[76] = new Vector2[4]
					{
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f)
					};
					points[77] = new Vector2[8]
					{
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0f, 0f),
						new Vector2(0.3f, -0.5f),
						new Vector2(0.6f, 0f),
						new Vector2(0.3f, -0.5f),
						new Vector2(0.6f, 0f),
						new Vector2(0.6f, -1f)
					};
					points[78] = new Vector2[6]
					{
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, 0f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, 0f)
					};
					points[79] = new Vector2[8]
					{
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, 0f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, 0f),
						new Vector2(0f, 0f)
					};
					points[80] = new Vector2[8]
					{
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0.6f, 0f),
						new Vector2(0.6f, -0.5f)
					};
					points[81] = new Vector2[10]
					{
						new Vector2(0.6f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, 0f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0.3f, -0.5f)
					};
					points[82] = new Vector2[10]
					{
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0.6f, 0f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0.15f, -0.5f),
						new Vector2(0.6f, -1f)
					};
					points[83] = new Vector2[10]
					{
						new Vector2(0f, 0f),
						new Vector2(0.6f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f)
					};
					points[84] = new Vector2[4]
					{
						new Vector2(0.6f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0.3f, -1f),
						new Vector2(0.3f, 0f)
					};
					points[85] = new Vector2[6]
					{
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, 0f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f)
					};
					points[86] = new Vector2[4]
					{
						new Vector2(0.3f, -1f),
						new Vector2(0f, 0f),
						new Vector2(0.3f, -1f),
						new Vector2(0.6f, 0f)
					};
					points[87] = new Vector2[8]
					{
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0f, -1f),
						new Vector2(0.3f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0.3f, -0.5f),
						new Vector2(0.6f, 0f),
						new Vector2(0.6f, -1f)
					};
					points[88] = new Vector2[4]
					{
						new Vector2(0.6f, -1f),
						new Vector2(0f, 0f),
						new Vector2(0.6f, 0f),
						new Vector2(0f, -1f)
					};
					points[89] = new Vector2[6]
					{
						new Vector2(0f, 0f),
						new Vector2(0.3f, -0.5f),
						new Vector2(0.6f, 0f),
						new Vector2(0.3f, -0.5f),
						new Vector2(0.3f, -1f),
						new Vector2(0.3f, -0.5f)
					};
					points[90] = new Vector2[6]
					{
						new Vector2(0.6f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0.6f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f)
					};
					points[91] = new Vector2[6]
					{
						new Vector2(0.4f, 0f),
						new Vector2(0.1f, 0f),
						new Vector2(0.1f, -1f),
						new Vector2(0.4f, -1f),
						new Vector2(0.1f, -1f),
						new Vector2(0.1f, 0f)
					};
					points[92] = new Vector2[2]
					{
						new Vector2(0.6f, -1f),
						new Vector2(0f, 0f)
					};
					points[93] = new Vector2[6]
					{
						new Vector2(0.2f, 0f),
						new Vector2(0.5f, 0f),
						new Vector2(0.2f, -1f),
						new Vector2(0.5f, -1f),
						new Vector2(0.5f, 0f),
						new Vector2(0.5f, -1f)
					};
					points[94] = new Vector2[4]
					{
						new Vector2(0f, -0.5f),
						new Vector2(0.3f, 0f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0.3f, 0f)
					};
					points[95] = new Vector2[2]
					{
						new Vector2(0f, -1f),
						new Vector2(0.8f, -1f)
					};
					points[96] = new Vector2[2]
					{
						new Vector2(0.5f, -0.3f),
						new Vector2(0.3f, 0f)
					};
					points[97] = new Vector2[10]
					{
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0f, -0.75f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -0.75f),
						new Vector2(0.6f, -0.75f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f)
					};
					points[98] = new Vector2[8]
					{
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, -0.5f)
					};
					points[99] = new Vector2[6]
					{
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f)
					};
					points[100] = new Vector2[8]
					{
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, 0f)
					};
					points[101] = new Vector2[10]
					{
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0.6f, -0.75f),
						new Vector2(0f, -0.75f),
						new Vector2(0.6f, -0.75f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f)
					};
					points[102] = new Vector2[8]
					{
						new Vector2(0.15f, -1f),
						new Vector2(0.15f, -0.25f),
						new Vector2(0.45f, 0f),
						new Vector2(0.3f, 0f),
						new Vector2(0.15f, -0.25f),
						new Vector2(0.3f, 0f),
						new Vector2(0.45f, -0.5f),
						new Vector2(0.15f, -0.5f)
					};
					points[103] = new Vector2[10]
					{
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -1.25f),
						new Vector2(0.6f, -1.25f),
						new Vector2(0.6f, -1.25f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -1f)
					};
					points[104] = new Vector2[6]
					{
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, -0.5f)
					};
					points[105] = new Vector2[4]
					{
						new Vector2(0.3f, -1f),
						new Vector2(0.3f, -0.5f),
						new Vector2(0.3f, -0.25f),
						new Vector2(0.3f, -0.15f)
					};
					points[106] = new Vector2[6]
					{
						new Vector2(0.3f, -0.25f),
						new Vector2(0.3f, -0.15f),
						new Vector2(0.3f, -1.25f),
						new Vector2(0.3f, -0.5f),
						new Vector2(0f, -1.25f),
						new Vector2(0.3f, -1.25f)
					};
					points[107] = new Vector2[6]
					{
						new Vector2(0f, -1f),
						new Vector2(0f, 0f),
						new Vector2(0f, -0.75f),
						new Vector2(0.3f, -0.5f),
						new Vector2(0f, -0.75f),
						new Vector2(0.6f, -1f)
					};
					points[108] = new Vector2[2]
					{
						new Vector2(0.3f, -1f),
						new Vector2(0.3f, 0f)
					};
					points[109] = new Vector2[10]
					{
						new Vector2(0.45f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.75f),
						new Vector2(0.45f, -0.5f),
						new Vector2(0.6f, -0.75f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0.3f, -1f),
						new Vector2(0.3f, -0.5f)
					};
					points[110] = new Vector2[8]
					{
						new Vector2(0.45f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.75f),
						new Vector2(0.45f, -0.5f),
						new Vector2(0.6f, -0.75f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f),
						new Vector2(0f, -0.5f)
					};
					points[111] = new Vector2[8]
					{
						new Vector2(0f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, -0.5f)
					};
					points[112] = new Vector2[8]
					{
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -1.25f),
						new Vector2(0f, -0.5f)
					};
					points[113] = new Vector2[8]
					{
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f),
						new Vector2(0f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -1.25f),
						new Vector2(0.6f, -0.5f)
					};
					points[114] = new Vector2[6]
					{
						new Vector2(0f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.75f),
						new Vector2(0.6f, -0.5f)
					};
					points[115] = new Vector2[10]
					{
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0f, -0.75f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.75f),
						new Vector2(0f, -0.75f),
						new Vector2(0.6f, -0.75f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f)
					};
					points[116] = new Vector2[6]
					{
						new Vector2(0.3f, -1f),
						new Vector2(0.3f, -0.25f),
						new Vector2(0.45f, -0.5f),
						new Vector2(0.15f, -0.5f),
						new Vector2(0.3f, -1f),
						new Vector2(0.45f, -1f)
					};
					points[117] = new Vector2[6]
					{
						new Vector2(0f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f)
					};
					points[118] = new Vector2[4]
					{
						new Vector2(0.3f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0.3f, -1f),
						new Vector2(0.6f, -0.5f)
					};
					points[119] = new Vector2[8]
					{
						new Vector2(0.15f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0.3f, -0.75f),
						new Vector2(0.15f, -1f),
						new Vector2(0.3f, -0.75f),
						new Vector2(0.45f, -1f),
						new Vector2(0.45f, -1f),
						new Vector2(0.6f, -0.5f)
					};
					points[120] = new Vector2[4]
					{
						new Vector2(0.6f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -0.5f)
					};
					points[121] = new Vector2[4]
					{
						new Vector2(0f, -1.25f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0.3f, -0.875f),
						new Vector2(0f, -0.5f)
					};
					points[122] = new Vector2[6]
					{
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f)
					};
				}
				return points;
			}
		}
	}
}
