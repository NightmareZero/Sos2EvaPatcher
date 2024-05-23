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

namespace Template
{
    internal class Sos2EvaPatchSettings : ModSettings
    {
        public List<ThingDef> eva = new List<ThingDef>();

        public override void ExposeData()
        {
            base.ExposeData();
            List<string> list = this.eva?.Select(selector: td => td.defName).ToList() ?? new List<string>();
            Scribe_Collections.Look(list: ref list, label: "sos2PatchedEvaList");
            this.eva = list.Select(selector: DefDatabase<ThingDef>.GetNamedSilentFail).Where(predicate: td => td != null).ToList();
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

        public override string SettingsCategory() => "EvaPatcher";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect: inRect);
            //     Listing_Standard listingStandard = new Listing_Standard();
            //     listingStandard.Begin(inRect);
            //     listingStandard.CheckboxLabeled("Disable Eva", ref settings.eva, "Disable Eva");
            //     listingStandard.End();
            //     settings.Write();
        }
    }


    [DefOf]
    public class TemplateDefOf
    {
        public static LetterDef success_letter;
    }

    public class MyMapComponent : MapComponent
    {
        public MyMapComponent(Map map) : base(map) { }
        public override void FinalizeInit()
        {
            Messages.Message("Success", null, MessageTypeDefOf.PositiveEvent);
            Find.LetterStack.ReceiveLetter(new TaggedString("Success"), new TaggedString("Success message"), TemplateDefOf.success_letter, "", 0);
        }
    }

    [StaticConstructorOnStartup]
    public static class Start
    {
        static Start()
        {
            Log.Message("Mod template loaded successfully!");

            // *Uncomment for Harmony*
            // Harmony harmony = new Harmony("Template");
            // harmony.PatchAll( Assembly.GetExecutingAssembly() );
        }
    }

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
