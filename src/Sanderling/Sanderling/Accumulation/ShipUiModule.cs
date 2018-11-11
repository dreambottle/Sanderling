using Bib3;
using Commons.Geometry;
using MemoryStruct = Sanderling.Interface.MemoryStruct;

namespace Sanderling.Accumulation
{
	public interface IShipUiModuleAndContext
	{
		MemoryStruct.IShipUiModule Module { get; }

		Vector2i? Location { get; }
	}

	public interface IShipUiModule : IEntityWithHistory<IShipUiModuleAndContext>, IEntityScoring<IShipUiModuleAndContext, Parse.IMemoryMeasurement>, MemoryStruct.IShipUiModule, IRepresentingMemoryObject
	{
		PropertyGenTimespanInt64<Parse.IModuleButtonTooltip> TooltipLast { get; }
	}
}
