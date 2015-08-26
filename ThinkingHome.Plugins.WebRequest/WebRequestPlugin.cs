using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ThinkingHome.Core.Plugins;
using ThinkingHome.Plugins.Scripts;

namespace ThinkingHome.Plugins.WebRequest
{
    [Plugin]
    public class WebRequestPlugin : PluginBase
    {
        // этот метод вызывается, когда сервис загружает плагины
        public override void InitPlugin()
        {
            Logger.Debug("init");
            base.InitPlugin();
        }

        // вызывается после того, как все плагины инициализированы
        public override void StartPlugin()
        {
            Logger.Debug("start");
            base.StartPlugin();
        }

        // вызывается при остановке сервиса
        public override void StopPlugin()
        {
            Logger.Debug("stop");
            base.StopPlugin();
        }

        [ScriptCommand("httpRequest")]
        public object ExecuteHttpCommand(string command)
        {
            string responseString;
            using (var client = new WebClient())
            {
                responseString = client.DownloadString(command);
            }
            Logger.Info(responseString);
            Logger.Info(command + " executed");
            return responseString;
        }
    }
}
