using System;
using System.Linq;
using Sanderling.Interface.MemoryStruct;
using Bib3;
using Commons.Geometry;

namespace Optimat.EveOnline.AuswertGbs
{
	public class SictAuswertGbsWindowChatChannel : SictAuswertGbsWindow
	{
		new static public WindowChatChannel BerecneFürWindowAst(
			UINodeInfoInTree windowAst)
		{
			if (null == windowAst)
				return null;

			var WindowAuswert = new SictAuswertGbsWindowChatChannel(windowAst);

			WindowAuswert.Berecne();

			return WindowAuswert.ErgeebnisScpezChatChannel;
		}

		public SictAuswertGbsListViewport<IListEntry> ViewportMessageAuswert
		{
			private set;
			get;
		}

		public SictAuswertGbsListViewport<IChatParticipantEntry> ViewportParticipantAuswert
		{
			private set;
			get;
		}

		public WindowChatChannel ErgeebnisScpezChatChannel
		{
			private set;
			get;
		}

		public SictAuswertGbsWindowChatChannel(UINodeInfoInTree windowStackAst)
			:
			base(windowStackAst)
		{
		}

		static public ChatParticipantEntry ListEntryParticipantConstruct(
			UINodeInfoInTree ast,
			IColumnHeader[] listHeader,
			RectInt? regionConstraint)
		{
			var ListEntry = SictAuswertGbsListViewport<IListEntry>.ListEntryKonstruktSctandard(ast, listHeader, regionConstraint);

			if (null == ListEntry)
				return null;

			var StatusIconSprite =
				ast.EnumerateChildNodeTransitiveHüle()
				?.OfType<UINodeInfoInTree>()
				?.FirstOrDefault(k => k.PyObjTypNameIsSprite() && (k.PyObjTypName?.ToLower().Contains("status") ?? false))
				?.AsSprite();

			var SetFlagWithStateIconNode =
				ast?.EnumerateChildNodeTransitiveHüle()
				?.OfType<UINodeInfoInTree>()
				?.Where(k => k?.PyObjTypNameMatchesRegexPatternIgnoreCase("FlagIconWithState") ?? false)
				?.ToArray();

			var SetFlagWithStateIcon =
				SetFlagWithStateIconNode
				?.Select(flagNode =>
				{
					var FlagIcon = flagNode.AsSprite();

					var childSprite =
						flagNode?.EnumerateChildNodeTransitiveHüle()?.OfType<UINodeInfoInTree>()
						?.Where(k => k.PyObjTypNameIsSprite())
						?.Select(k => k?.AsSprite())?.WhereNotDefault()?.FirstOrDefault();

					if (null != FlagIcon)
					{
						FlagIcon.Texture0Id = FlagIcon.Texture0Id ?? childSprite?.Texture0Id;
						FlagIcon.TexturePath = FlagIcon.TexturePath ?? childSprite?.TexturePath;
						FlagIcon.Color = FlagIcon.Color ?? childSprite?.Color;
					}

					return FlagIcon;
				})
				?.WhereNotDefault()
				?.ToArrayIfNotEmpty();

			return new ChatParticipantEntry(ListEntry)
			{
				NameLabel = ast.LargestLabelInSubtree().AsUIElementTextIfTextNotEmpty(),
				StatusIcon = StatusIconSprite,
				FlagIcon = SetFlagWithStateIcon,
			};
		}

		override public void Berecne()
		{
			base.Berecne();

			var AstMainContainer = base.AstMainContainer;

			if (null == AstMainContainer)
				return;

			if (!(true == AstMainContainer.VisibleIncludingInheritance))
				return;

			var mainContainerCenter = AstMainContainer.AsUIElementIfVisible().RegionCenter();

			var scrollNodesWithRegionCenter =
				AstMainContainer.MatchingNodesFromSubtreeBreadthFirst(node => node.PyObjTypNameIsScroll())
				?.Select(node => (node, regionCenter: node.AsUIElementIfVisible().RegionCenter()))
				?.ToList();

			//	Assume we find the container of the messages on the left from the center of the window.
			var ViewportSetMessageAst =
				scrollNodesWithRegionCenter
				?.FirstOrDefault(nodeWithCenter => nodeWithCenter.regionCenter?.X < mainContainerCenter?.X).node;

			//	Assume we find the container of the participants on the right from the center of the window.
			var ViewportSetParticipantAst =
				scrollNodesWithRegionCenter
				?.FirstOrDefault(nodeWithCenter => mainContainerCenter?.X < nodeWithCenter.regionCenter?.X).node;

			var FunkIsOther = new Func<IObjectIdInMemory, bool>(obj =>
			  !(ViewportSetMessageAst?.EnthaltAstMitHerkunftAdrese(obj.Id) ?? false) &&
			  !(ViewportSetParticipantAst?.EnthaltAstMitHerkunftAdrese(obj.Id) ?? false));

			var LabelOther =
				AstMainContainer.ExtraktMengeLabelString()
				?.Where(label => FunkIsOther(label))
				?.OrderByLabel()
				?.ToArray();

			if (null != ViewportSetMessageAst)
			{
				ViewportMessageAuswert = new SictAuswertGbsListViewport<IListEntry>(ViewportSetMessageAst, SictAuswertGbsListViewport<IListEntry>.ListEntryKonstruktSctandard);
				ViewportMessageAuswert.Read();
			}

			if (null != ViewportSetParticipantAst)
			{
				ViewportParticipantAuswert = new SictAuswertGbsListViewport<IChatParticipantEntry>(ViewportSetParticipantAst, ListEntryParticipantConstruct);
				ViewportParticipantAuswert.Read();
			}

			var MessageInputAst =
				AstMainContainer
				?.MatchingNodesFromSubtreeBreadthFirst(node => node?.PyObjTypNameMatchesRegexPattern("EditPlainText") ?? false)
				?.OrderByDescending(node => node.Size?.SqrMagnitude ?? -1)
				?.FirstOrDefault();

			ErgeebnisScpezChatChannel = new WindowChatChannel(
				base.Ergeebnis)
			{
				MessageView = ViewportMessageAuswert?.Result,
				ParticipantView = ViewportParticipantAuswert?.Result,
				MessageInput = MessageInputAst?.AsUIElementInputText(),
			};
		}
	}

}
