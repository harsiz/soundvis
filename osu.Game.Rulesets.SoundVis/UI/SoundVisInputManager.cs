using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.SoundVis.UI
{
    // RulesetInputManager supports replay recording, PassThroughInputManager does not.
    // Player.SetRecordTarget blows up if it finds a PassThroughInputManager.
    public partial class SoundVisInputManager : RulesetInputManager<SoundVisAction>
    {
        public SoundVisInputManager(RulesetInfo? ruleset)
            : base(ruleset!, 0, SimultaneousBindingMode.Unique)
        {
        }
    }
}
