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
        // first inited in game
        public bool inited = false;

        // enable eva patcher
        public bool enabled = false;

        // patch all apparel with eva tag to eva suit
        public bool patchEvaTag = false;
        // all patched eva suit
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
            this.patchEvaTag = false; 
            this.eva = new List<ThingDef>();
        }
    }

    internal class Sos2EvaPatchMod : Mod
    {
        #region variables
        public static Sos2EvaPatchSettings settings;
        public static Sos2EvaPatchMod Instance;
        #endregion

        #region ui components
        private string searchTerm = "";

        private ThingDef leftSelectedItem;
        private ThingDef rightSelectedItem;

        private Vector2 leftScrollPosition;
        private Vector2 rightScrollPosition;
        #endregion
        
        public Sos2EvaPatchMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<Sos2EvaPatchSettings>();
            // Instance = this;
        }

        public override string SettingsCategory() => "EvaPatcherGeneralSettings";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect: inRect);
            Text.Font = GameFont.Medium;

            // get all apparel
            List<ThingDef> allApparel = DefDatabase<ThingDef>.AllDefs.Where(predicate: x => x.IsApparel).ToList();

            #region topRect
            Rect topRect = inRect.TopPart(pct: 0.05f);
            Listing_Standard ls = new Listing_Standard();
            ls.Begin(topRect);
            ls.Label("EvaPatcherGeneralSettings".Translate());
            ls.GapLine(20f);
            // enable eva patcher
            ls.Label("EnableEvaPatcher".Translate());
            ls.CheckboxLabeled("EnableEvaPatcher".Translate(), ref settings.enabled, "EnableEvaPatcher".Translate());
            // set all appearel has eva tag to eva suit
            ls.Label("UseEvatag".Translate());
            ls.CheckboxLabeled("UseEvatag".Translate(), ref settings.enabled, "UseEvatag".Translate());
            ls.GapLine(20f);
            this.searchTerm = Widgets.TextField(rect: topRect.RightPart(pct: 0.95f).LeftPart(pct: 0.95f), text: this.searchTerm);
            
            ls.End();
            #endregion

            #region leftRect
            Rect leftRect = inRect.BottomPart(pct: 0.45f).LeftPart(pct: 0.5f);
            GUI.BeginGroup(position: leftRect, style: new GUIStyle(other: GUI.skin.box));
            // get left list item
            List<ThingDef> leftList = allApparel.Where(x=> !settings.eva.Contains(x))
            .Where(x=> x.apparel.tags.Contains(DefValue.EvaTagName))
            .Where(x=>  x.label.Contains(this.searchTerm))
            .ToList();

            float leftItemY = 3f;
            Widgets.BeginScrollView(outRect: leftRect.AtZero(), scrollPosition: ref this.leftScrollPosition,
                                    viewRect: new Rect(x: 0f, y: 0f, width: leftRect.width / 10 * 9, height: leftList.Count * 32f));
            if (!leftList.NullOrEmpty())
            {
                foreach (ThingDef def in leftList)
                {
                    Rect rowRect = new Rect(x: 5, y: leftItemY, width: leftRect.width - 6, height: 30);
                    Widgets.DrawHighlightIfMouseover(rect: rowRect);
                    if (def == this.leftSelectedItem)
                        Widgets.DrawHighlightSelected(rect: rowRect);
                    Widgets.Label(rect: rowRect, label: def.LabelCap.RawText ?? def.defName);
                    if (Widgets.ButtonInvisible(butRect: rowRect))
                        this.leftSelectedItem = def;

                    leftItemY += 32f;
                }
            }

            Widgets.EndScrollView();
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
