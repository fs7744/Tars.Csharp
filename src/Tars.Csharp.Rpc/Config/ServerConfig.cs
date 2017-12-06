using System;
using System.Collections.Generic;
using System.IO;

namespace Tars.Csharp.Rpc.Config
{
    public class ServerConfig
    {
        public ClientConfig ClientConfig { get; private set; }
        public string Application { get; private set; }
        public string ServerName { get; private set; }
        public Endpoint Local { get; private set; }
        public string Node { get; private set; }
        public string BasePath { get; private set; }
        public string DataPath { get; private set; }
        public string CharsetName { get; private set; }
        public string Config { get; private set; }
        public string Notify { get; private set; }
        public string Log { get; private set; }
        public string LogPath { get; private set; }
        public string LogLevel { get; private set; }
        public int LogRate { get; private set; }
        public string LocalIP { get; private set; }
        public int SessionTimeOut { get; private set; }
        public int SessionCheckInterval { get; private set; }
        public int UdpBufferSize { get; private set; }
        public bool TcpNoDelay { get; private set; }
        public Dictionary<string, ServantAdapterConfig> ServantAdapterConfMap { get; private set; }

        public ServerConfig Load(TarsConfig conf)
        {
            Application = conf.Get("/tars/application/server<app>", "UNKNOWN");
            ServerName = conf.Get("/tars/application/server<server>", null);

            string localStr = conf.Get("/tars/application/server<local>");
            Local = localStr == null || localStr.Length <= 0 ? null : Endpoint
                    .ParseString(localStr);
            Node = conf.Get("/tars/application/server<node>");
            BasePath = conf.Get("/tars/application/server<basepath>");
            DataPath = conf.Get("/tars/application/server<datapath>");

            CharsetName = conf.Get("/tars/application/server<charsetname>", "UTF-8");

            Config = conf.Get("/tars/application/server<config>");
            Notify = conf.Get("/tars/application/server<notify>");

            Log = conf.Get("/tars/application/server<log>");
            LogPath = conf.Get("/tars/application/server<logpath>", null);
            LogLevel = conf.Get("/tars/application/server<loglevel>");
            LogRate = conf.GetInt("/tars/application/server<lograte>", 5);

            LocalIP = conf.Get("/tars/application/server<localip>");

            SessionTimeOut = conf.GetInt(
                    "/tars/application/server<sessiontimeout>", 120000);
            SessionCheckInterval = conf.GetInt(
                    "/tars/application/server<sessioncheckinterval>", 60000);
            UdpBufferSize = conf.GetInt("/tars/application/server<udpbuffersize>",
                    4096);
            TcpNoDelay = conf
                    .GetBool("/tars/application/server<tcpnodelay>", false);

            ServantAdapterConfMap = new Dictionary<string, ServantAdapterConfig>();
            List<String> adapterNameList = conf
                    .GetSubTags("/tars/application/server");
            if (adapterNameList != null)
            {
                foreach (string adapterName in adapterNameList)
                {
                    ServantAdapterConfig config = new ServantAdapterConfig();
                    config.Load(conf, adapterName);
                    ServantAdapterConfMap.Add(config.Servant, config);
                }
            }

            ServantAdapterConfig adminServantAdapterConfig = new ServantAdapterConfig();
            adminServantAdapterConfig.Endpoint = Local;
            adminServantAdapterConfig.Servant = $"{Application}.{ServerName}.{Constants.AdminServant}";
            ServantAdapterConfMap.Add(Constants.AdminServant,
                    adminServantAdapterConfig);

            if (Application != null && ServerName != null && LogPath != null)
            {
                LogPath = LogPath + Path.PathSeparator + Application + Path.PathSeparator
                        + ServerName;
            }
            ClientConfig = new ClientConfig().Load(conf);
            if (LogPath != null)
            {
                ClientConfig.LogPath = LogPath;
            }
            ClientConfig.LogLevel = LogLevel;
            ClientConfig.DataPath = DataPath;
            return this;
        }

        public static ServerConfig ParseFrom()
        {
            return new ServerConfig().Load(TarsConfig.ParseFile(Environment.GetEnvironmentVariable("config")));
        }
    }
}