using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Bib3;
using Commons.Geometry;
using Sanderling.Interface.MemoryStruct;

namespace Optimat.EveOnline.AuswertGbs
{
	static public class Extension
	{
		static public Vector2i AsVector2i(
			this Vector2f vector2f) =>
			new Vector2i((int)vector2f.X, (int)vector2f.Y);

		static readonly Bib3.RefNezDiferenz.SictTypeBehandlungRictliinieMitTransportIdentScatescpaicer
			KonvertGbsAstInfoRictliinieMitScatescpaicer =
			new Bib3.RefNezDiferenz.SictTypeBehandlungRictliinieMitTransportIdentScatescpaicer(
				Bib3.RefNezDiferenz.NewtonsoftJson.SictMengeTypeBehandlungRictliinieNewtonsoftJson.KonstruktMengeTypeBehandlungRictliinie(
				new KeyValuePair<Type, Type>[]{
					new KeyValuePair<Type, Type>(typeof(GbsNodeInfo), typeof(UINodeInfoInTree)),
					new KeyValuePair<Type, Type>(typeof(GbsNodeInfo[]), typeof(UINodeInfoInTree[])),
				}));

		static public UINodeInfoInTree SictAuswert(
			this GbsNodeInfo gbsBaum)
		{
			if (null == gbsBaum)
				return null;

            //var treeCopy = new Bib3.RefBaumKopii.Param(null, KonvertGbsAstInfoRictliinieMitScatescpaicer);

            var GbsBaumScpez =
				SictRefNezKopii.ObjektKopiiErsctele(
                    gbsBaum,
				    null,
                    new Bib3.RefBaumKopii.Param(null, KonvertGbsAstInfoRictliinieMitScatescpaicer),
				    null,
				    null)
				as UINodeInfoInTree;

			if (null == GbsBaumScpez)
				return null;

			int InBaumAstIndexZääler = 0;

			GbsBaumScpez.AbgelaiteteAigescafteBerecne(ref InBaumAstIndexZääler);

			return GbsBaumScpez;
		}

        /// Senseo snapshot?
		static public IMemoryMeasurement SensorikScnapscusKonstrukt(
			this Optimat.EveOnline.GbsNodeInfo gbsBaum,
			int? sessionDurationRemaining)
		{
			var GbsBaumSictAuswert = gbsBaum.SictAuswert();

			var Auswert = new SictAuswertGbsAgr(GbsBaumSictAuswert);

			Auswert.Berecne(sessionDurationRemaining);

			return Auswert.AuswertErgeebnis;
		}


		static public IEnumerable<UINodeInfoInTree> BaumEnumFlacListeKnoote(
			this UINodeInfoInTree suuceWurzel,
			int? tiifeMax = null,
			int? tiifeMin = null)
		{
			return
				suuceWurzel.EnumerateNodeFromTreeBFirst(
				node => node?.GetChildList()?.OfType<UINodeInfoInTree>(),
				tiifeMax,
				tiifeMin);
		}

		static public Vector2f? LaagePlusVonParentErbeLaage(
			this UINodeInfoInTree node)
		{
			var VonParentErbeLaage = node?.FromParentLocation;

			if (!VonParentErbeLaage.HasValue)
				return node.PositionInParent;

			return node.PositionInParent + VonParentErbeLaage;
		}

		static public string LabelText(
			this UINodeInfoInTree node) => node?.SetText;

		static public void AbgelaiteteAigescafteBerecne(
			this UINodeInfoInTree node,
			ref int inBaumAstIndexZääler,
			int? tiifeMax = null,
			Vector2f? vonParentErbeLaage = null,
			float? vonParentErbeClippingFläceLinx = null,
			float? vonParentErbeClippingFläceOobn = null,
			float? vonParentErbeClippingFläceRecz = null,
			float? vonParentErbeClippingFläceUntn = null)
		{
			if (null == node)
				return;

			if (tiifeMax < 0)
				return;

			node.InTreeIndex = ++inBaumAstIndexZääler;
			node.FromParentLocation = vonParentErbeLaage;

			var FürChildVonParentErbeLaage = node.PositionInParent;

			var LaagePlusVonParentErbeLaage = node.LaagePlusVonParentErbeLaage();
			var Grööse = node.Size;

			var FürChildVonParentErbeClippingFläceLinx = vonParentErbeClippingFläceLinx;
			var FürChildVonParentErbeClippingFläceOobn = vonParentErbeClippingFläceOobn;
			var FürChildVonParentErbeClippingFläceRecz = vonParentErbeClippingFläceRecz;
			var FürChildVonParentErbeClippingFläceUntn = vonParentErbeClippingFläceUntn;

			if (LaagePlusVonParentErbeLaage.HasValue && Grööse.HasValue)
			{
				FürChildVonParentErbeClippingFläceLinx = Bib3.Glob.Max(FürChildVonParentErbeClippingFläceLinx, LaagePlusVonParentErbeLaage.Value.X);
				FürChildVonParentErbeClippingFläceRecz = Bib3.Glob.Min(FürChildVonParentErbeClippingFläceRecz, LaagePlusVonParentErbeLaage.Value.X);
				FürChildVonParentErbeClippingFläceOobn = Bib3.Glob.Max(FürChildVonParentErbeClippingFläceOobn, LaagePlusVonParentErbeLaage.Value.Y);
				FürChildVonParentErbeClippingFläceUntn = Bib3.Glob.Min(FürChildVonParentErbeClippingFläceUntn, LaagePlusVonParentErbeLaage.Value.Y);
			}

			if (vonParentErbeLaage.HasValue)
			{
				if (FürChildVonParentErbeLaage.HasValue)
				{
					FürChildVonParentErbeLaage = FürChildVonParentErbeLaage.Value + vonParentErbeLaage.Value;
				}
				else
				{
					FürChildVonParentErbeLaage = vonParentErbeLaage;
				}
			}

			var ListeChild = node.Children;

			for (int ChildIndex = 0; ChildIndex < ListeChild?.Length; ChildIndex++)
			{
				var Child = ListeChild[ChildIndex];

				if (null == Child)
					continue;

				Child.InParentListChildIndex = ChildIndex;
				Child.AbgelaiteteAigescafteBerecne(
					ref inBaumAstIndexZääler,
					tiifeMax - 1,
					FürChildVonParentErbeLaage,
					FürChildVonParentErbeClippingFläceLinx,
					FürChildVonParentErbeClippingFläceOobn,
					FürChildVonParentErbeClippingFläceRecz,
					FürChildVonParentErbeClippingFläceUntn);
			}

			var MengeChildInBaumAstIndex =
				ListeChild
				?.Select(child => child?.ChildLastInTreeIndex ?? child?.InTreeIndex)
				?.WhereNotDefault()
				?.ToArray();

			if (0 < MengeChildInBaumAstIndex?.Length)
			{
				node.ChildLastInTreeIndex = MengeChildInBaumAstIndex.Max();
			}
		}

		static public UINodeInfoInTree SuuceFlacMengeAstFrüheste(
			this UINodeInfoInTree[] suuceMengeWurzel,
			Func<UINodeInfoInTree, bool> prädikaat,
			int? tiifeMax = null,
			int? tiifeMin = null)
		{
			foreach (var Wurzel in suuceMengeWurzel.EmptyIfNull())
			{
				var Fund = Wurzel.FirstMatchingNodeFromSubtreeBreadthFirst(prädikaat, tiifeMax, tiifeMin);

				if (null != Fund)
					return Fund;
			}

			return null;
		}

		static public T Grööste<T>(
			this IEnumerable<T> source)
			where T : class, IUIElement =>
			source?.OrderByDescending(element => element.Region.Area())?.FirstOrDefault();

		static public T LargestNodeInSubtree<T>(
			this IEnumerable<T> source)
			where T : GbsNodeInfo =>
			source?.OrderByDescending(element => (element.SizeA * element.SizeB) ?? int.MinValue)?.FirstOrDefault();

		static public T GröösteSpriteAst<T>(
			this IEnumerable<T> source)
			where T : GbsNodeInfo =>
			source?.Where(k => k.PyObjTypNameIsSprite())
			?.LargestNodeInSubtree();

		static public UINodeInfoInTree LargestLabelInSubtree(
			this UINodeInfoInTree rootNode,
			int? tiifeMax = null)
		{
			var mengeLabelSictbar =
				rootNode.MatchingNodesFromSubtreeBreadthFirst(kandidaat => kandidaat.GbsAstTypeIstLabel(), null, tiifeMax);

			UINodeInfoInTree bisherBeste = null;

			foreach (var LabelAst in mengeLabelSictbar.EmptyIfNull())
			{
				var labelAstGrööse = LabelAst?.Size;

				if (!labelAstGrööse.HasValue)
					continue;

				if ((bisherBeste?.Size.Value.SqrMagnitude ?? -1) < labelAstGrööse.Value.SqrMagnitude)
					bisherBeste = LabelAst;
			}

			return bisherBeste;
		}

		static public UINodeInfoInTree[] MatchingNodesFromSubtreeBreadthFirst(
			this UINodeInfoInTree rootNode,
			Func<UINodeInfoInTree, bool> predicate,
			int? resultCountMax = null,
			int? depthBoundMax = null,
			int? depthBoundMin = null,
			bool omitNodesBelowNodesMatchingPredicate = false)	=>
			rootNode.ListPathToNodeFromSubtreeBreadthFirst(
				predicate,
				resultCountMax,
				depthBoundMax,
				depthBoundMin,
				omitNodesBelowNodesMatchingPredicate)
				?.Select(astMitPfaad => astMitPfaad.LastOrDefault()).ToArray();

		static public UINodeInfoInTree FirstNodeWithPyObjAddressFromSubtreeBreadthFirst(
			this UINodeInfoInTree node,
			Int64? pyObjAddress,
			int? depthBoundMax = null,
			int? depthBoundMin = null) =>
			node.FirstMatchingNodeFromSubtreeBreadthFirst(
				kandidaat => kandidaat.PyObjAddress == pyObjAddress,
				depthBoundMax,
				depthBoundMin);

		static public UINodeInfoInTree FirstMatchingNodeFromSubtreeBreadthFirst(
			this UINodeInfoInTree rootNode,
			Func<UINodeInfoInTree, bool> predicate,
			int? depthBoundMax = null,
			int? depthBoundMin = null) =>
			rootNode?.MatchingNodesFromSubtreeBreadthFirst(predicate, 1, depthBoundMax, depthBoundMin, true)
			?.FirstOrDefault();

		static public UINodeInfoInTree[][] ListPathToNodeFromSubtreeBreadthFirst(
			this UINodeInfoInTree rootNode,
			Func<UINodeInfoInTree, bool> predicate,
			int? ListeFundAnzaalScrankeMax = null,
			int? depthBoundMax = null,
			int? depthBoundMin = null,
			bool omitNodesBelowNodesMatchingPredicate = false)
		{
			if (null == rootNode)
				return null;

			return Bib3.Glob.SuuceFlacMengeAstMitPfaad(
				rootNode,
				predicate,
				node => node.Children,
				ListeFundAnzaalScrankeMax,
				depthBoundMax,
				depthBoundMin,
				omitNodesBelowNodesMatchingPredicate);
		}

		static public Vector2f? SizeMaxOfListChildren(
			this UINodeInfoInTree node)
		{
			if (null == node)
			{
				return null;
			}

			Vector2f? sizeMax = null;

			var thisSize = node.Size;

			if (thisSize.HasValue)
			{
				sizeMax = thisSize;
			}

			var ListeChild = node.Children;

			if (null != ListeChild)
			{
				foreach (var Child in ListeChild)
				{
					var ChildSize = Child.Size;

					if (ChildSize.HasValue)
					{
						if (sizeMax.HasValue)
						{
							sizeMax = new Vector2f(
								Math.Max(sizeMax.Value.X, ChildSize.Value.X),
								Math.Max(sizeMax.Value.Y, ChildSize.Value.Y));
						}
						else
						{
							sizeMax = ChildSize;
						}
					}
				}
			}

			return sizeMax;
		}

		static string[] UIRootSizeListNames = new string[] { "l_main", "l_viewstate" };

		static public Vector2f? SizeOfListChildForSpecUIRootBranch(
			this UINodeInfoInTree node)
		{
			if (null == node)
			{
				return null;
			}

			var ListeChild = node.Children;

			if (null != ListeChild)
			{
				foreach (var Child in ListeChild)
				{
					var ChildSize = Child.Size;

					if (ChildSize.HasValue)
					{
						if (UIRootSizeListNames.Any((nodeName) => string.Equals(nodeName, Child.Name)))
						{
							return ChildSize;
						}
					}
				}
			}

			return null;
		}

		static public IUIElementText AsUIElementText(this UINodeInfoInTree GbsAst) =>
			(GbsAst?.VisibleIncludingInheritance ?? false) ? new UIElementText(GbsAst.AsUIElementIfVisible(), GbsAst.LabelText() ?? GbsAst.Text) : null;

		static public IUIElementInputText AsUIElementInputText(this UINodeInfoInTree GbsAst)
		{
			var UIElementText = GbsAst?.AsUIElementText();

			return null == UIElementText ? null : new UIElementInputText(UIElementText);
		}

		static public IUIElementText AsUIElementTextIfTextNotEmpty(
			this UINodeInfoInTree GbsAst)
		{
			var UIElementText = GbsAst?.AsUIElementText();

			if ((UIElementText?.Text).IsNullOrEmpty())
				return null;

			return UIElementText;
		}

		static public IEnumerable<IUIElementText> ExtraktMengeLabelString(
			this UINodeInfoInTree GbsAst) =>
			GbsAst?.MatchingNodesFromSubtreeBreadthFirst(kandidaat => kandidaat?.VisibleIncludingInheritance ?? false)
			?.Select(AsUIElementTextIfTextNotEmpty)
			?.WhereNotDefault();

		static public IEnumerable<IUIElementText> ExtraktMengeButtonLabelString(
			this UINodeInfoInTree GbsAst) =>
			GbsAst?.MatchingNodesFromSubtreeBreadthFirst(kandidaat => (kandidaat?.VisibleIncludingInheritance ?? false) &&
			Regex.Match(kandidaat?.PyObjTypName ?? "", "button", RegexOptions.IgnoreCase).Success)
			?.Select(kandidaatButtonAst => new { ButtonAst = kandidaatButtonAst, LabelAst = kandidaatButtonAst.LargestLabelInSubtree() })
			?.GroupBy(buttonAstUndLabelAst => buttonAstUndLabelAst.LabelAst)
			?.Select(GroupLabelAst => new
			{
				ButtonAst = GroupLabelAst.Select(buttonAstUndLabelAst => buttonAstUndLabelAst.ButtonAst).OrderBy(kandidaatButtonAst => kandidaatButtonAst.InTreeIndex).LastOrDefault(),
				LabelAst = GroupLabelAst.Key
			})
			?.Select(buttonAstUndLabelAst => new UIElementText(buttonAstUndLabelAst.ButtonAst.AsUIElementIfVisible(),
				buttonAstUndLabelAst?.LabelAst?.LabelText()))
			?.Where(kandidaat => !(kandidaat?.Text).IsNullOrEmpty());

		static public IEnumerable<T> OrderByLabel<T>(
			this IEnumerable<T> labels)
			where T : IUIElement =>
			labels
			?.OrderBy(element => ((element?.Region)?.Center())?.Y ?? int.MaxValue)
			?.ThenBy(element => ((element?.Region)?.Center())?.X ?? int.MaxValue);

		static public Sprite AsSprite(
			this UINodeInfoInTree gbsNode) =>
			!(gbsNode?.VisibleIncludingInheritance ?? false) ? null :
			new Sprite(gbsNode.AsUIElementIfVisible())
			{
				Name = gbsNode?.Name,
				Color = gbsNode.Color,
				Texture0Id = gbsNode?.TextureIdent0?.AsObjectIdInMemory(),
				HintText = gbsNode?.Hint,
				TexturePath = gbsNode?.texturePath,
			};

		static public ListViewAndControl<EntryT> AsListView<EntryT>(
			this UINodeInfoInTree ListViewportAst,
			Func<UINodeInfoInTree, IColumnHeader[], RectInt?, EntryT> CallbackListEntryConstruct = null,
			ListEntryTrenungZeleTypEnum? InEntryTrenungZeleTyp = null)
			where EntryT : class, IListEntry
		{
			var Auswert = new SictAuswertGbsListViewport<EntryT>(
				ListViewportAst,
				CallbackListEntryConstruct,
				InEntryTrenungZeleTyp);

			Auswert.Read();

			return Auswert?.Result;
		}

		static public IUIElement WithRegionConstrainedToIntersection(
			this IUIElement original,
			RectInt constraint) =>
			original?.WithRegion(original.Region.Intersection(constraint));

		static public Container AsContainer(
			this UINodeInfoInTree containerNode,
			bool treatIconAsSprite = false,
			RectInt? regionConstraint = null)
		{
			if (!(containerNode?.VisibleIncludingInheritance ?? false))
				return null;

			var MengeKandidaatInputTextAst =
				containerNode?.MatchingNodesFromSubtreeBreadthFirst(k =>
					k.PyObjTypNameMatchesRegexPatternIgnoreCase("SinglelineEdit|QuickFilterEdit"));

			var ListeInputText =
				MengeKandidaatInputTextAst
				?.Select(textBoxAst =>
				{
					var LabelAst = textBoxAst.LargestLabelInSubtree();

					if (null == LabelAst)
						return null;

					var LabelText = LabelAst?.LabelText();

					return new UIElementInputText(textBoxAst.AsUIElementIfVisible(), LabelText);
				})
				?.WhereNotDefault()
				?.OrderByLabel()
				?.ToArrayIfNotEmpty();

			var ListeButton =
				containerNode?.ExtraktMengeButtonLabelString()?.OrderByLabel()
				?.ToArrayIfNotEmpty();

			var ListeButtonAst = ListeButton?.Select(button => containerNode.FirstNodeWithPyObjAddressFromSubtreeBreadthFirst(button.Id))?.ToArray();

			var ListeTextBoxAst = ListeInputText?.Select(textBox => containerNode.FirstNodeWithPyObjAddressFromSubtreeBreadthFirst(textBox.Id))?.ToArray();

			var LabelContainerAussclus = new[] { ListeButtonAst, ListeTextBoxAst }.ConcatNullable().ToArray();

			var ListeLabelText =
				containerNode?.ExtraktMengeLabelString()
				?.WhereNitEnthalte(LabelContainerAussclus)
				?.OrderByLabel()
				?.ToArrayIfNotEmpty();

			var setSprite =
				containerNode.SetSpriteFromChildren(treatIconAsSprite)
				?.OrderByLabel()
				?.ToArrayIfNotEmpty();

			var baseElement = containerNode.AsUIElementIfVisible();

			if (regionConstraint.HasValue)
				baseElement = baseElement.WithRegionConstrainedToIntersection(regionConstraint.Value);

			return new Container(baseElement)
			{
				ButtonText = ListeButton,
				InputText = ListeInputText,
				LabelText = ListeLabelText,
				Sprite = setSprite,
			};
		}

		static public IEnumerable<ISprite> SetSpriteFromChildren(
			this UINodeInfoInTree uiNode,
			bool treatIconAsSprite = false) =>
			uiNode?.MatchingNodesFromSubtreeBreadthFirst(c =>
				(c?.PyObjTypNameIsSprite() ?? false) ||
				(treatIconAsSprite && (c?.PyObjTypNameIsIcon() ?? false)), null, null, null, true)
				?.Select(spriteNode => spriteNode?.AsSprite())
				?.WhereNotDefault();

		static public IInSpaceBracket AsInSpaceBracket(this UINodeInfoInTree node)
		{
			var container = node?.AsContainer();

			if (null == container)
				return null;

			return new InSpaceBracket(container)
			{
				Name = node.Name,
			};
		}

		/// <summary>
		/// Diis werd verwand mit LayerUtilmenu, daher prüüfung Sictbarkait nit ausraicend.
		/// </summary>
		/// <param name="GbsAst"></param>
		/// <returns></returns>
		static public IContainer AlsUtilmenu(this UINodeInfoInTree GbsAst)
		{
			//	2015.09.08:	PyObjTypName	= "ExpandedUtilMenu"

			var AstExpanded =
				GbsAst?.FirstMatchingNodeFromSubtreeBreadthFirst(k => k.PyObjTypNameMatchesRegexPatternIgnoreCase("ExpandedUtilMenu"));

			return AstExpanded?.AsContainer();
		}

		static public Func<Int64, IEnumerable<KeyValuePair<string, SictAuswertPythonObj>>> FunkEnumDictEntry;

		static public IEnumerable<T> WhereNitEnthalte<T, AstT>(
			this IEnumerable<T> MengeKandidaat,
			IEnumerable<AstT> MengeContainerZuMaide)
			where T : IObjectIdInMemory
			where AstT : GbsNodeInfo =>
			MengeKandidaat?.Where(Kandidaat => !(MengeContainerZuMaide?.Any(ContainerZuMaide =>
			new AstT[] { ContainerZuMaide }.ConcatNullable(ContainerZuMaide.EnumerateChildNodeTransitiveHüle()).Any(ContainerZuMaideChild => ContainerZuMaideChild.PyObjAddress == Kandidaat.Id)) ?? false));

		static public IEnumerable<NodeT> OrderByRegionSizeDescending<NodeT>(this IEnumerable<NodeT> seq)
			where NodeT : GbsNodeInfo => seq?.OrderByDescending(node => node?.Size?.Magnitude ?? -1);
	}
}
