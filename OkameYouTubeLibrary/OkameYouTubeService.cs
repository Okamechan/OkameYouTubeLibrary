using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OkameYouTubeLibrary
{
    public class OkameYouTubeService
    {
        private YouTubeService _service;

        public OkameYouTubeService(string apiKey)
        {
            _service = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = apiKey
            });
        }

        /// <summary>
        /// LiveChatMessageをQueueに入れるためのループを自動で発生させる
        /// </summary>
        /// <param name="queue">LiveChatMessageを入れるためのQueue</param>
        /// <param name="liveChatId"></param>
        /// <param name="token"></param>
        public void GetLiveChatMessageLoop(Queue<LiveChatMessage> queue,string videoId, CancellationToken token)
        {
            Task.Run(async () =>
            {
                string liveChatId = Utils.GetliveChatID(videoId, _service);
                await Utils.GetLiveChatMessage(queue, liveChatId, _service, null, token);
            });
        }
    }
}
