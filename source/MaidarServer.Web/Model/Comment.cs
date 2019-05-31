﻿using MaiDar.Core;
using MaiDar.Core.Model.Enumerations;
using MaiDarServer.CMS.Core;
using MaiDarServer.CMS.Model;
using MaiDarServer.Plugin.Models;

namespace MaiDarServer.API.Model
{
    public class Comment
    {
        public Comment(CommentInfo commentInfo, IUserInfo userInfo)
        {
            if (commentInfo == null) return;

            Id = commentInfo.Id;
            AddDate = DateUtils.GetDateAndTimeString(commentInfo.AddDate, EDateFormatType.Chinese, ETimeFormatType.ShortTime);
            GoodCount = commentInfo.GoodCount;
            Content = commentInfo.Content;
            IsChecked = commentInfo.IsChecked;
            DisplayName = userInfo?.DisplayName;
            AvatarUrl = PageUtility.GetUserAvatarUrl(PageUtils.OuterApiUrl, userInfo);
        }

        public int Id { get; set; }

        public string AddDate { get; set; }

        public int GoodCount { get; set; }

        public string Content { get; set; }

        public bool IsChecked { get; set; }

        public string DisplayName { get; set; }

        public string AvatarUrl { get; set; }
    }
}