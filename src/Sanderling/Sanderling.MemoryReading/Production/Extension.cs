using Optimat.EveOnline;
using Optimat.EveOnline.AuswertGbs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sanderling.MemoryReading.Production
{
    static public class Extension
    {
        //// view type treatment/processing guideline/directive  ... shadow? storey?
        //static readonly Bib3.RefNezDiferenz.SictTypeBehandlungRictliinieMitTransportIdentScatescpaicer
        //    KonvertGbsAstInfoRictliinieMitScatescpaicer =
        //    new Bib3.RefNezDiferenz.SictTypeBehandlungRictliinieMitTransportIdentScatescpaicer(
        //        Bib3.RefNezDiferenz.NewtonsoftJson.SictMengeTypeBehandlungRictliinieNewtonsoftJson.KonstruktMengeTypeBehandlungRictliinie(
        //        new KeyValuePair<Type, Type>[]{
        //            new KeyValuePair<Type, Type>(typeof(GbsNodeInfo), typeof(UINodeInfoInTree)),
        //            new KeyValuePair<Type, Type>(typeof(GbsNodeInfo[]), typeof(UINodeInfoInTree[])),
        //}));
        
        static public IEnumerable<T[]> EnumerateNodePathsInTree<T>(
            this T searchRoot)
            where T : GbsNodeInfo =>

            Bib3.Extension.EnumeratePathToNodeFromTree(searchRoot, node => node.GetChildList()?.OfType<T>());
        
        static public T[] FindFirstInTreeWhere<T>(
            this T searchRoot,
            Func<T, bool> predicate)
            where T : GbsNodeInfo =>
            EnumerateNodePathsInTree(searchRoot)
            ?.Where(path => predicate(path?.LastOrDefault()))
            ?.FirstOrDefault();

    }
}
