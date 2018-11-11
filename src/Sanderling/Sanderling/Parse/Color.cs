using Commons.Struct;

namespace Sanderling.Parse
{
	static public class ColorExtension
	{
		static public bool IsRed(this ArgbColor color) =>
			null != color &&
			color.BMilli < color.RMilli / 3 &&
			color.GMilli < color.RMilli / 3 &&
			300 < color.RMilli;

	}
}
