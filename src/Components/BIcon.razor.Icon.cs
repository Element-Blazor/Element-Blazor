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
		[Display(Prompt ="platform-eleme")]
		PlatformEleme = 0,
		[Display(Prompt = "user")]
		User = 1,
		[Display(Prompt = "circle-plus-outline")]
		CirclePlusOutline = 2
	}
}
