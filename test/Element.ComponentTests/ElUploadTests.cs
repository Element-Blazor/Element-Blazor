using Bunit;
using Element;
using Element.ControlConfigs;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Element.ComponentTests
{
    public class ElUploadTests : BunitContext
    {
        public ElUploadTests()
        {
            Services.AddElementServices();
        }

        [Fact]
        public async Task PictureCardListRendersActionsAndTemplateContent()
        {
            RenderFragment<UploadModel> template = file => builder =>
            {
                builder.OpenElement(0, "span");
                builder.AddAttribute(1, "class", "custom-file");
                builder.AddContent(2, $"{file.FileName}:{file.Status}");
                builder.CloseElement();
            };
            var cut = Render<ElUpload>(parameters => parameters
                .Add(x => x.Url, "/upload")
                .Add(x => x.EnablePasteUpload, false)
                .Add(x => x.ListType, UploadListType.PictureCard)
                .Add(x => x.AutoUpload, false)
                .Add(x => x.FileTemplate, template));

            await cut.InvokeAsync(() => cut.Instance.PasteUploadFiles(new[]
            {
                new[] { "avatar.png", "1024", "64", "64", "/avatar.png" }
            }));

            Assert.Contains("el-upload--picture-card", cut.Find(".el-upload").ClassList);
            Assert.Contains("el-upload-list--picture-card", cut.Find(".el-upload-list").ClassList);
            Assert.Equal("avatar.png:UnStart", cut.Find(".custom-file").TextContent);
        }

        [Fact]
        public void DragUploadRendersDraggerAndAcceptsConfiguredExtensions()
        {
            var cut = Render<ElUpload>(parameters => parameters
                .Add(x => x.Url, "/upload")
                .Add(x => x.EnablePasteUpload, false)
                .Add(x => x.Drag, true)
                .Add(x => x.AllowExtensions, new[] { ".png", ".jpg" }));

            var root = cut.Find(".el-upload");
            var input = cut.Find("input[type=file]");

            Assert.Contains("el-upload--drag", root.ClassList);
            Assert.NotNull(cut.Find(".el-upload-dragger"));
            Assert.Equal(".png,.jpg", input.GetAttribute("accept"));
        }

        [Fact]
        public async Task LimitInvokesExceedAndKeepsExistingFiles()
        {
            UploadExceedEventArgs exceed = null;
            var cut = Render<ElUpload>(parameters => parameters
                .Add(x => x.Url, "/upload")
                .Add(x => x.EnablePasteUpload, false)
                .Add(x => x.AutoUpload, false)
                .Add(x => x.Limit, 1)
                .Add(x => x.OnExceed, args => exceed = args));

            await cut.InvokeAsync(() => cut.Instance.PasteUploadFiles(new[]
            {
                new[] { "one.txt", "100" }
            }));
            await cut.InvokeAsync(() => cut.Instance.PasteUploadFiles(new[]
            {
                new[] { "two.txt", "100" }
            }));

            Assert.NotNull(exceed);
            Assert.Equal("two.txt", exceed.Files.Single().FileName);
            Assert.Single(cut.Instance.Files);
            Assert.Equal("true", cut.Find(".el-upload").GetAttribute("aria-disabled"));
        }

        [Fact]
        public async Task BeforeUploadPreviewAndBeforeRemoveHooksRun()
        {
            var changed = new List<string>();
            IFileModel previewed = null;
            var deleteCount = 0;
            var cut = Render<ElUpload>(parameters => parameters
                .Add(x => x.Url, "/upload")
                .Add(x => x.EnablePasteUpload, false)
                .Add(x => x.AutoUpload, false)
                .Add(x => x.BeforeUpload, file => Task.FromResult(file.FileName != "blocked.txt"))
                .Add(x => x.BeforeRemove, _ => Task.FromResult(false))
                .Add(x => x.OnPreview, file => previewed = file)
                .Add(x => x.OnDeleteFile, _ => deleteCount++)
                .Add(x => x.OnChange, args => changed.Add(args.File.FileName)));

            await cut.InvokeAsync(() => cut.Instance.PasteUploadFiles(new[]
            {
                new[] { "blocked.txt", "100" },
                new[] { "accepted.txt", "100" }
            }));

            cut.Find(".el-upload-list__item-name").Click();
            cut.Find(".el-icon-close").Click();

            Assert.Equal(new[] { "accepted.txt" }, changed);
            Assert.Equal("accepted.txt", previewed.FileName);
            Assert.Equal(0, deleteCount);
            Assert.Single(cut.Instance.Files);
        }

        [Fact]
        public async Task CustomRequestControlsUploadResultAndProgress()
        {
            JSInterop.SetupVoid("clear", _ => true).SetVoidResult();
            var progress = new List<double>();
            UploadRequestContext request = null;
            UploadRequestEventArgs success = null;
            IFileModel[] uploadedList = null;
            var cut = Render<ElUpload>(parameters => parameters
                .Add(x => x.Url, "/custom-upload")
                .Add(x => x.EnablePasteUpload, false)
                .Add(x => x.AutoUpload, false)
                .Add(x => x.Method, "put")
                .Add(x => x.FileFieldName, "asset")
                .Add(x => x.HttpRequest, async context =>
                {
                    request = context;
                    await context.ReportProgress(42);
                    return UploadRequestResult.Success("server-id", "/files/report.txt", "ok");
                })
                .Add(x => x.OnProgress, args => progress.Add(args.Percent))
                .Add(x => x.OnSuccess, args => success = args)
                .Add(x => x.OnFileListUpload, files => uploadedList = files));

            await cut.InvokeAsync(() => cut.Instance.PasteUploadFiles(new[]
            {
                new[] { "report.txt", "2048" }
            }));
            await cut.InvokeAsync(() => cut.Instance.SubmitAsync());

            var file = cut.Instance.Files.Cast<UploadModel>().Single();
            Assert.Equal("report.txt", request.File.FileName);
            Assert.Equal("/custom-upload", request.Url);
            Assert.Equal("put", request.Method);
            Assert.Equal("asset", request.FileFieldName);
            Assert.Equal(new[] { 0d, 42d, 100d }, progress);
            Assert.NotNull(success);
            Assert.Equal("server-id", file.Id);
            Assert.Equal("/files/report.txt", file.Url);
            Assert.Equal(UploadStatus.Success, file.Status);
            Assert.Single(uploadedList);
            Assert.Contains("is-success", cut.Find(".el-upload-list__item").ClassList);
        }

        [Fact]
        public void GeneratedUploadAppliesAttributeConfiguration()
        {
            var cut = Render<ElForm>(parameters => parameters
                .Add(x => x.EntityType, typeof(GeneratedUploadModel))
                .Add(x => x.Value, new GeneratedUploadModel()));

            var root = cut.Find(".el-upload");
            var input = cut.Find("input[type=file]");

            Assert.Contains("el-upload--picture-card", root.ClassList);
            Assert.Contains("is-disabled", root.ClassList);
            Assert.Equal("true", root.GetAttribute("aria-disabled"));
            Assert.Equal(".png", input.GetAttribute("accept"));
            Assert.Equal("asset", input.GetAttribute("name"));
            Assert.False(input.HasAttribute("multiple"));
            Assert.Empty(cut.FindAll(".el-upload-list"));
        }

        private class GeneratedUploadModel
        {
            [Upload(
                Url = "/upload",
                ListType = UploadListType.PictureCard,
                Limit = 1,
                Multiple = false,
                AutoUpload = false,
                ShowFileList = false,
                Disabled = true,
                Accept = ".png",
                FileFieldName = "asset",
                EnablePasteUpload = false)]
            public IFileModel[] Files { get; set; }
        }
    }
}
