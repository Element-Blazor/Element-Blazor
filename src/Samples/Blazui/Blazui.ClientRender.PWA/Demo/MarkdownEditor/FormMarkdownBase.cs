using Element;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element.ClientRender.PWA.Demo.MarkdownEditor
{
    public class FormMarkdownBase : ElementComponentBase
    {
        internal BForm form;

        internal ArticleModel article;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            article = new ArticleModel()
            {
                Title = "测试",
                Content = "# 测试"
            };
        }

        protected void Submit()
        {
            if (!form.IsValid())
            {
                return;
            }
            Alert(form.GetValue<ArticleModel>().Content);
        }
    }
}
