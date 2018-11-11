using System;
using System.Collections.Generic;
using BotEngine.Interface;
using Commons.Geometry;
using Commons.Struct;

namespace Optimat.EveOnline
{
	public class InGbsPath
	{
		public Int64? rootNodeAddress;

		public Int64[] nodeAddressList;

		public InGbsPath()
		{
		}

		public InGbsPath(
			Int64? rootNodeAddress,
			Int64[] nodeAddressList = null)
		{
			this.rootNodeAddress = rootNodeAddress;
			this.nodeAddressList = nodeAddressList;
		}
	}

	public class GbsNodeInfo
	{
        /// <summary>
        /// Address from which the branch was read from target process.
        /// Adrese von welcer der Ast aus Ziil Proces geleese wurde.
        /// </summary>
        public Int64? PyObjAddress;

		public string PyObjTypName;

		public bool? VisibleIncludingInheritance;

		public string Name;

		public string Text;

		public string SetText;

		public string LinkText;

		public string Hint;

		public float? PositionInParentA;

		public float? PositionInParentB;

		public float? SizeA;

		public float? SizeB;

		public string Caption;

		public bool? Minimized;

		public bool? isModal;

		public bool? isSelected;

		public string WindowID;

		public double? LastStateFloat;

		public double? LastSetCapacitorFloat;

		/// <summary>
		/// 2015.08.26
		/// Beobactung in Type ShipHudSpriteGauge mit sclüsl "_lastValue" und type "float" (verwandt jewails für Shield, Armor, Struct)
		/// </summary>
		public double? LastValueFloat;

		public double? RotationFloat;

		public string SrHtmlstr;

		public string EditTextlineCoreText;

		public int? ColorAMili;

		public int? ColorRMili;

		public int? ColorGMili;

		public int? ColorBMili;

		/// <summary>
		/// 2013.09.13 Pfaad: [(Dict["_texture"] as trinity.Tr2Sprite2dTexture).[8] + 80]
		/// </summary>
		public Int64? TextureIdent0;

		public string texturePath;

		public float? Speed;

		public float? CapacitorLevel;

		public float? ShieldLevel;

		public float? ArmorLevel;

		public float? StructureLevel;

		public GbsNodeInfo[] Children;

		public GbsNodeInfo[] BackgroundList;

		public string[] DictListKeyStringValueNotEmpty;

		public int? SquadronSize;

		public int? SquadronMaxSize;

		public bool? RampActive;

		public ArgbColor Color
		{
			set
			{
				AssignToComponents(value, ref ColorAMili, ref ColorRMili, ref ColorGMili, ref ColorBMili);
			}

			get
			{
				return CreateColorArgbFromComponents(ColorAMili, ColorRMili, ColorGMili, ColorBMili);
			}
		}

		public Vector2f? PositionInParent
		{
			set
			{
				assignToComponents(value, ref PositionInParentA, ref PositionInParentB);
			}

			get
			{
				return ComponentToVector2f(PositionInParentA, PositionInParentB);
			}
		}

		public Vector2f? Size
		{
			set
			{
				assignToComponents(value, ref SizeA, ref SizeB);
			}

			get
			{
				return ComponentToVector2f(SizeA, SizeB);
			}
		}

		public GbsNodeInfo()
		{
		}

		public GbsNodeInfo(
			Int64? inProzesHerkunftAdrese)
		{
			this.PyObjAddress = inProzesHerkunftAdrese;
		}

		virtual public IEnumerable<GbsNodeInfo> GetChildList()
		{
			return Children;
		}

		static public void assignToComponents(
			Vector2f? vector,
			ref float? componentX,
			ref float? componentY)
		{
			componentX = vector?.X;
			componentY = vector?.Y;
		}

		static public Vector2f? ComponentToVector2f(
			float? a,
			float? b)
		{
			if (!a.HasValue || !b.HasValue)
				return null;

			return new Vector2f(a.Value, b.Value);
		}

		static public void AssignToComponents(
			ArgbColor color,
			ref int? componentAMili,
			ref int? componentRMili,
			ref int? componentGMili,
			ref int? componentBMili)
		{
			componentAMili = color?.AMilli;
			componentRMili = color?.RMilli;
			componentGMili = color?.GMilli;
			componentBMili = color?.BMilli;
		}

		static public ArgbColor CreateColorArgbFromComponents(
			int? aMilli,
			int? rMilli,
			int? gMilli,
			int? bMilli)
		{
			return new ArgbColor(aMilli, rMilli, gMilli, bMilli);
		}

		public IEnumerable<GbsNodeInfo> EnumerateChildNodeTransitiveHüle(
			int? depthMaxLimit = null)
		{
			var childList = this.GetChildList();

			if (depthMaxLimit <= 0)
				return null;

			if (null == childList)
				return null;

			var childListTransitive = new List<GbsNodeInfo>();

			foreach (var child in childList)
			{
				if (null == child)
					continue;

				childListTransitive.Add(child);

				var childrenOfTheChild = child.EnumerateChildNodeTransitiveHüle(depthMaxLimit - 1);
                
				if (null != childrenOfTheChild)
					childListTransitive.AddRange(childrenOfTheChild);
			}

			return childListTransitive;
		}

		public Int64[] EnumerateSelfAndChildNodeHerkunftAdreseTransitiiveHüleBerecne(
			int? depthMaxLimit = null)
		{
			var MengeAdrese = new List<Int64>();

			MengeSelbsctUndChildAstHerkunftAdreseTransitiiveHüleFüügeAinNaacListe(MengeAdrese, depthMaxLimit);

			return MengeAdrese.ToArray();
		}

		public void MengeSelbsctUndChildAstHerkunftAdreseTransitiiveHüleFüügeAinNaacListe(
			IList<Int64> ziilListe,
			int? tiifeScrankeMax = null)
		{
			if (null == ziilListe)
				return;

			var herkunftAdrese = this.PyObjAddress;

			if (herkunftAdrese.HasValue)
				ziilListe.Add(herkunftAdrese.Value);

			var listeChild = this.GetChildList();

			if (tiifeScrankeMax <= 0)
				return;

			if (null == listeChild)
				return;

			var listeChildMengeChildAstHerkunftAdreseTransitiiv = new List<Int64>();

			foreach (var Child in listeChild)
			{
				if (null == Child)
					continue;

				Child.MengeSelbsctUndChildAstHerkunftAdreseTransitiiveHüleFüügeAinNaacListe(
					ziilListe,
					tiifeScrankeMax - 1);
			}
		}
	}
}
