//如果好用，请收藏地址，帮忙分享。
using System.Collections.Generic;
namespace Game
{


    public class ResourcesItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string mxgameId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int itemType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string itemTypeName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string itemId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int duration { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int redeemed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int expireAt { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string conditionId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int conditionNum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int coins { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int cash { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long startTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long stopTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string description { get; set; }
    }

    public class PropsData
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string nextUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ResourcesItem> resources { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string refreshUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string lastUrl { get; set; }
    }
}