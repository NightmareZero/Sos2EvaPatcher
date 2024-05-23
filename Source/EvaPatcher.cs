using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;
using Verse.Noise;
using Verse.Grammar;
using RimWorld;
using RimWorld.Planet;

// *Uncomment for Harmony*
// using System.Reflection;
using HarmonyLib;

namespace EvaPatcher
{
    internal static class DefValue
    {
        internal const String EvaTagName = "EVA";
    }

    internal class Sos2EvaPatchSettings : ModSettings
    {

        #region modConfig
        public bool inited = false;

        public bool enabled = false;
        public List<ThingDef> eva = new List<ThingDef>();
        #endregion

        public override void ExposeData()
        {
            base.ExposeData();
            if (!this.inited)
            {
                this.InitData();
            }
            List<string> list = this.eva?.Select(selector: td => td.defName).ToList() ?? new List<string>();
            // load config
            Scribe_Collections.Look(list: ref list, label: "EvaPatchedList");
            Scribe_Values.Look(value: ref inited, label: "EvaPatchedInited", defaultValue: false);
            this.eva = list.Select(selector: DefDatabase<ThingDef>.GetNamedSilentFail).Where(predicate: td => td != null).ToList();
        }

        public void InitData()
        {
            this.inited = true;
            this.enabled = false;
            this.eva = new List<ThingDef>();
        }
    }

    internal class Sos2EvaPatcher : Mod
    {
        public static Sos2EvaPatchSettings settings;
        public Sos2EvaPatcher(ModContentPack content) : base(content)
        {
            settings = GetSettings<Sos2EvaPatchSettings>();
            // Instance = this;
        }

        public override string SettingsCategory() => "EvaPatcherGeneralSettings";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect: inRect);
            Text.Font = GameFont.Medium;

            #region topRect
            Rect topRect = inRect.TopPart(pct: 0.05f);
            Listing_Standard ls = new Listing_Standard();
            ls.Begin(topRect);
            ls.Label("EvaPatcherGeneralSettings".Translate());
            ls.GapLine(20f);
            ls.Label("Enable EvaPatcher".Translate());
            ls.CheckboxLabeled("Enable EvaPatcher".Translate(), ref settings.enabled, "Enable EvaPatcher".Translate());
            ls.GapLine(20f);
            //     ls.CheckboxLabeled("Disable Eva", ref settings.eva, "Disable Eva");
            ls.End();
            #endregion

            #region leftRect
            Rect leftRect = inRect.BottomPart(pct: 0.45f).LeftPart(pct: 0.5f);
            GUI.BeginGroup(position: leftRect, style: new GUIStyle(other: GUI.skin.box));

            GUI.EndGroup();
            #endregion

            #region rightRect
            Rect rightRect = inRect.BottomPart(pct: 0.45f).RightPart(pct: 0.5f);
            GUI.BeginGroup(position: rightRect, style: new GUIStyle(other: GUI.skin.box));

            GUI.EndGroup();
            #endregion

            settings.Write();
        }
    }

    [StaticConstructorOnStartup]
    public static class EvaPatcher
    {
        static EvaPatcher()
        {
            Log.Message("Mod template loaded successfully!");

            // *Uncomment for Harmony*
            Harmony harmony = new Harmony("rimworld.nightz.sos2EvaPatcher");
            // harmony.Patch(original: AccessTools.Method(type: typeof(Apparel),name: ApparelUtility.), prefix: null, postfix: new HarmonyMethod(typeof(EvaPatcher).GetMethod("EvaApparelPostfix")));
            // harmony.PatchAll( Assembly.GetExecutingAssembly() );
        }
    }
    // [DefOf]
    // public class TemplateDefOf
    // {
    //     public static LetterDef success_letter;
    // }

    // public class MyMapComponent : MapComponent
    // {
    //     public MyMapComponent(Map map) : base(map) { }
    //     public override void FinalizeInit()
    //     {
    //         Messages.Message("Success", null, MessageTypeDefOf.PositiveEvent);
    //         Find.LetterStack.ReceiveLetter(new TaggedString("Success"), new TaggedString("Success message"), TemplateDefOf.success_letter, "", 0);
    //     }
    // }


    // *Uncomment for Harmony*
    // [HarmonyPatch(typeof(LetterStack), "ReceiveLetter")]
    // [HarmonyPatch(new Type[] {typeof(TaggedString), typeof(TaggedString), typeof(LetterDef), typeof(string), typeof(int), typeof(bool)})]
    // public static class LetterTextChange
    // {
    //     public static bool Prefix(ref TaggedString text)
    //     {
    //         text += new TaggedString(" with harmony");
    //         return true;
    //     }
    // }

}
