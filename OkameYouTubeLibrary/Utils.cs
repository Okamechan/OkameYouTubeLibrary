using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OkameYouTubeLibrary
{
    internal static class Utils
    {
        /// <summary>
        /// LiveChatIDを取得する
        /// </summary>
        /// <param name="videoId">配信のリンクのv=のあとに続く文字列</param>
        /// <param name="service">YouTubeServiceのインスタンス</param>
        /// <returns>LiveChatID、失敗はnull</returns>
        internal static string GetliveChatID(string videoId, YouTubeService service)
        { 
            // 配信を選択
            var videosList = service.Videos.List("LiveStreamingDetails");
            videosList.Id = videoId;
            
            //動画情報の取得
            var videoListResponse = videosList.Execute();

            //LiveChatIDを返す
            foreach (var videoID in videoListResponse.Items)
            {
                return videoID.LiveStreamingDetails.ActiveLiveChatId;
            }
            
            // ない場合はnullが返る
            return null;
        }

        /// <summary>
        /// LiveChatMessageをリストに入れる、Taskは再帰を行うのでawaitするとずっと止まる
        /// </summary>
        /// <param name="list">変更するリスト</param>
        /// <param name="liveChatId"></param>
        /// <param name="service"></param>
        /// <param name="nextPageToken"></param>
        /// <param name="isGetStack"></param>
        /// <returns></returns>
        internal static async Task GetLiveChatMessage(Queue<LiveChatMessage> queue, string liveChatId, YouTubeService service, string nextPageToken, CancellationToken token, bool isGetStack = false)
        {
            if (token.IsCancellationRequested)
                return;

            // リクエストの作成
            var liveChatRequest = service.LiveChatMessages.List(liveChatId, "snippet,authorDetails");
            liveChatRequest.PageToken = nextPageToken;

            // レスポンスの受け取り
            var liveChatResponse = await liveChatRequest.ExecuteAsync();

            // チャットの読み取り
            if (isGetStack)
            {
                foreach (var liveChat in liveChatResponse.Items)
                {
                    try
                    {
                        queue.Enqueue(liveChat);
                    }
                    catch { }

                }
            }
            await Task.Delay((int)liveChatResponse.PollingIntervalMillis);

            await GetLiveChatMessage(queue, liveChatId, service, liveChatResponse.NextPageToken, token);
        }
    }
}
