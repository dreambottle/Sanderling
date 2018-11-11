using Commons.Struct;
using Sanderling.Interface.MemoryStruct;

namespace Optimat.EveOnline.AuswertGbs
{
	public class SictAuswertGbsTab
	{
		readonly public UINodeInfoInTree TabAst;

		public UINodeInfoInTree LabelClipperAst
		{
			private set;
			get;
		}

		public UINodeInfoInTree LabelNode
		{
			private set;
			get;
		}

		public string LabelText
		{
			private set;
			get;
		}

		public ArgbColor LabelColor
		{
			private set;
			get;
		}

		public Tab Ergeebnis
		{
			private set;
			get;
		}

		public SictAuswertGbsTab(UINodeInfoInTree tabAst)
		{
			this.TabAst = tabAst;
		}

		virtual public void Berecne()
		{
			if (null == TabAst)
				return;

			if (!(true == TabAst.VisibleIncludingInheritance))
				return;

			LabelNode = TabAst.LargestLabelInSubtree(3);

			if (null == LabelNode)
				return;

			LabelColor = LabelNode.Color;
			LabelText = LabelNode.LabelText();

			if (null == LabelText || null == LabelColor)
				return;

			var LabelColorOpacityMilli = LabelNode.ColorAMili;

			var Label = new UIElementText(LabelNode.AsUIElementIfVisible(), LabelText);

			Ergeebnis = new Tab(TabAst.AsUIElementIfVisible())
			{
				Label = Label,
				LabelColorOpacityMilli = LabelColorOpacityMilli,
			};
		}
	}
}
