using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Blazui.Component
{
    /// <summary>
    /// https://element.eleme.cn/#/zh-CN/component/icon
    /// </summary>
    public enum Icon
    {
        [Display(Prompt = "platform-eleme")]
        PlatformEleme = 0,
        [Display(Prompt = "user")]
        User = 1,
        [Display(Prompt = "circle-plus-outline")]
        CirclePlusOutline = 2,
        [Display(Prompt = "edit-outline")]
        EditOutline = 3,
        [Display(Prompt = "document-delete")]
        DocumentDelete = 4,
        [Display(Prompt = "video-play")]
        VideoPlay = 5,
        [Display(Prompt = "video-pause")]
        VideoPause = 6,
        [Display(Prompt = "switch-button")]
        SwitchButton = 7,
        [Display(Prompt = "delete")]
        Delete = 8,
        [Display(Prompt = "close")]
        Close = 9
    }
}
