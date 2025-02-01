using MTM101BaldAPI;

namespace TweaksPlus.Comps
{
	public class ConditionalPatchConfigWithDesc(string category, string name, string desc, bool defaultValue) : ConditionalPatch
	{
		private bool _dfVal = defaultValue;
		private string _category = category;
		private string _name = name;
		private string _desc = desc;

		public override bool ShouldPatch() =>
			Plugin.plug.Config.Bind<bool>(_category, _name, _dfVal, _desc).Value;
		
	}
}
