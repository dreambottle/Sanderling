using BotEngine.Interface;
using Commons.Struct;

namespace Sanderling.Compat
{
	static public class ColorCompat
	{
		static public ArgbColor AsArgbColor(this ColorORGBVal? color)
		{
			return new ArgbColor(color?.OMilli, color?.RMilli, color?.GMilli, color?.BMilli);
		}
	}
}
