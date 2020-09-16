using Blazui.Component.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.ServerRender.Demo.Tree
{
    [Route("api/tree")]
    public class TreeController : ControllerBase
    {
        [HttpGet]
        [Route("{nodeid}")]
        public List<TreeItemModel> Get(int? nodeId)
        {
            if (nodeId == null)
            {
                return new List<TreeItemModel>
                {
                    new TreeItemModel()
                    {
                         HasChildren=true,
                         Id=1,
                          Text="节点1"
                    },
                    new TreeItemModel()
                    {
                        HasChildren=false,
                         Id=2,
                         Text="节点2"
                    },
                    new TreeItemModel()
                    {
                        HasChildren=false,
                        Id=3,
                         ParentId=1,
                          Text="节点3"
                    }
                };
            }
            return new List<TreeItemModel>();
        }
    }
}
