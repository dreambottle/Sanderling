using System.Linq;
using Sanderling.Interface.MemoryStruct;
using System.Text.RegularExpressions;
using System;
using Commons.Geometry;

namespace Optimat.EveOnline.AuswertGbs
{
	public class SictAuswertGbsMenu
	{
		public const string MenuEntryPyTypeName = "MenuEntryView";

		static public Menu ReadMenu(UINodeInfoInTree menuNode)
		{
			if (!(menuNode?.VisibleIncludingInheritance ?? false))
				return null;

			var setEntryNode =
				menuNode.MatchingNodesFromSubtreeBreadthFirst(
				kandidaat => kandidaat?.PyObjTypNameMatchesRegexPatternIgnoreCase(MenuEntryPyTypeName) ?? false,
				null, 3, 1);

			var baseElement = menuNode.AsUIElementIfVisible();

			var setEntry =
				setEntryNode
				?.Select(kandidaatAst => ReadMenuEntry(kandidaatAst, baseElement?.Region ?? RectInt.Empty)).ToArray();

			var listEntry = setEntry?.OrderByLabel()?.ToArray();

			return new Menu(baseElement)
			{
				Entry = listEntry,
			};
		}

		static public MenuEntry ReadMenuEntry(
			UINodeInfoInTree entryNode,
			RectInt regionConstraint)
		{
			if (!(entryNode?.VisibleIncludingInheritance ?? false))
				return null;

			var fillNode =
				entryNode.FirstMatchingNodeFromSubtreeBreadthFirst(kandidaat => string.Equals("Fill", kandidaat.PyObjTypName, StringComparison.InvariantCultureIgnoreCase), 2, 1) ??
				entryNode.FirstMatchingNodeFromSubtreeBreadthFirst(kandidaat => Regex.Match(kandidaat.PyObjTypName ?? "", "Underlay", RegexOptions.IgnoreCase).Success, 2, 1);

			var entryHighlight =
				null != fillNode?.ColorAMili ? (200 < fillNode?.ColorAMili) : (bool?)null;

			return entryNode.MenuEntry(regionConstraint, entryHighlight);
		}
	}
}
