using Decal.Adapter;
using Decal.Adapter.Wrappers;
using System;
using System.Reflection;
using static VCS5.PluginCore;

namespace Hyperlinks.Integrations
{
    internal static class VCSProxy
    {


        public static PluginHost Host = null;

        /// <summary>
        /// A shortcut method to initialize plugin name and the PluginHost object.
        /// </summary>
        /// <param name="myhost">PluginCore.Host</param>
        /// <param name="mypluginname">The friendly name of this plugin. Used in the presets list.</param>
        public static void Initialize(PluginHost myhost)
        {
            Host = myhost;
        }

        #region SendChatText


        static void Curtain_SendChatTextVCS(string text, int color, int window)
        {
            VCS5.PluginCore.Instance.FilterOutputText(text, window, color);
        }

        static void Curtain_SendChatTextVViews(string text, int color, int vvsfilteras)
        {
            VirindiViewService.Controls.HudChatbox.SendChatText(text, (VirindiViewService.Controls.HudConsole.eACTextColor)color, (VirindiViewService.eConsoleColorClass)vvsfilteras);
        }

        #endregion SendChatText

        #region Sending Categorized Text

        /// <summary>
        /// Send a filtered chat message by VCS preset. Call Initialize() first, then call InitializeCategory() to
        /// create the preset, then finally call this to output text.
        /// </summary>
        /// <param name="categoryname">The preset name. Should already be initialized by InitializeCategory().</param>
        /// <param name="text">The output chat text.</param>
        /// <param name="color">The default AC console color ID.</param>
        /// <param name="windows">The default target windows, 0=auto, 1=main, 2=float1</param>
        public static void SendChatTextCategorized(string categoryname, string text, int color, params int[] windows)
        {
            if ((windows == null) || (windows.Length == 0)) windows = new int[] { 1 };


          

            if (IsVCSPresent(Host))
            {
                Curtain_SendChatTextCategorized(categoryname, text, color, windows);
            }
            else
            {
                foreach (int x in windows)
                {
                    if (Host != null)
                        Host.Actions.AddChatText(text, color, x);
                    else
                        CoreManager.Current.Actions.AddChatText(text, color, x);
                }
            }

            if (IsVirindiViewsPresent(Host))
                Curtain_SendChatTextVViews(text, color, (int)eVVSConsoleColorClass.PluginMessage);
        }

        public static void SendPluginText(string text, int color = 0)
        {
            SendChatTextCategorized("CommandLine", "[" + DecalPlugin.PluginName + "] " + text, color);
        }
        static void Curtain_SendChatTextCategorized(string categoryname, string text, int color, params int[] windows)
        {
            VCS5.Presets.FilterOutputPreset(DecalPlugin.PluginName, categoryname, text, color, windows);
        }

        /// <summary>
        /// Creates a VCS preset type which can later be used for chat. Will appear in the VCS presets list. Call Initialize() first.
        /// </summary>
        /// <param name="categoryname">The preset name.</param>
        /// <param name="description">The preset description.</param>
        public static void InitializeCategory(string categoryname, string description)
        {
            if (IsVCSPresent(Host))
                Curtain_InitializeCategory(categoryname, description);
        }

        static void Curtain_InitializeCategory(string categoryname, string description)
        {
            VCS5.Presets.RegisterPreset(DecalPlugin.PluginName, categoryname, description);
        }

        #endregion Sending Categorized Text

        #region VCS and VVS online checks

        static bool seenvcsassembly = false;

        public static bool IsVCSPresent(PluginHost pHost)
        {
            try
            {
                //See if VCS assembly is loaded
                if (!seenvcsassembly)
                {
                    System.Reflection.Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (System.Reflection.Assembly a in asms)
                    {
                        AssemblyName nmm = a.GetName();
                        if ((nmm.Name == "VCS5") && (nmm.Version >= new System.Version("5.0.0.5")))
                        {
                            seenvcsassembly = true;
                            break;
                        }
                    }
                }

                if (seenvcsassembly)
                    if (Curtain_VCSInstanceEnabled())
                        return true;
            }
            catch
            {

            }

            return false;
        }

        static bool Curtain_VCSInstanceEnabled()
        {
            return VCS5.PluginCore.Running;
        }

        static bool has_cachedvvsresult = false;
        static bool cachedvvsresult = false;

        //Doh
        //Need to know about VVS to post to VVS "console" controls.
        //Since VVS is a service and can't be turned on and off at runtime, we only need to do this once.
        public static bool IsVirindiViewsPresent(PluginHost pHost)
        {
            try
            {
                if (has_cachedvvsresult) return cachedvvsresult;

                //See if VCS assembly is loaded
                System.Reflection.Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
                bool l = false;
                foreach (System.Reflection.Assembly a in asms)
                {
                    AssemblyName nmm = a.GetName();
                    if ((nmm.Name == "VirindiViewService") && (nmm.Version >= new System.Version("1.0.0.14")))
                    {
                        l = true;
                        break;
                    }
                }

                if (l)
                    if (Curtain_VirindiViewsInstanceEnabled())
                    {
                        has_cachedvvsresult = true;
                        cachedvvsresult = true;
                        return true;
                    }
            }
            catch
            {

            }

            has_cachedvvsresult = true;
            cachedvvsresult = false;
            return false;
        }

        static bool Curtain_VirindiViewsInstanceEnabled()
        {
            return VirindiViewService.Service.Running;
        }

        #endregion VCS and VVS online checks

    }
}
