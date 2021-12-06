using Decal.Adapter;
using Hyperlinks.Integrations;
using Hyperlinks.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hyperlinks
{
    [FriendlyName(DecalPlugin.PluginName)]
    public sealed class DecalPlugin : PluginBase
    {
        public static DecalPlugin Current { get; private set; }
        public const string PluginName = "Hyperlinks";
        private CoreManager _coreManager { get { return CoreManager.Current; } }


        protected override void Startup()
        {
            _coreManager.ChatBoxMessage += _coreManager_ChatBoxMessage;
            _coreManager.ChatNameClicked += _coreManager_ChatNameClicked;
        }

        private void _coreManager_ChatNameClicked(object sender, ChatClickInterceptEventArgs e)
        {
            if (e.Id != 8675 || !StringExtensions.ContainsUrl(e.Text))
                return;

            if (e.Text.ToLowerInvariant().EndsWith(".bat") || e.Text.ToLowerInvariant().EndsWith(".exe"))
                return;

            e.Eat = true;
            System.Diagnostics.Process.Start(e.Text);

        }

        private void _coreManager_ChatBoxMessage(object sender, ChatTextInterceptEventArgs e)
        {

            if (!StringExtensions.ContainsUrl(e.Text))
                return;

            e.Eat = true;
            var newMessage = StringExtensions.ApplyUrlLinks(e.Text);
            VCSProxy.SendChatTextCategorized("CommandLine", newMessage, e.Color);
        }




        protected override void Shutdown()
        {
            Current = null;
        }


    }
}
