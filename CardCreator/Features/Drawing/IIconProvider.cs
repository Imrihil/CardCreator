using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace CardCreator.Features.Drawing
{
    public interface IIconProvider
    {
        Image Get(string name);
        Image TryGet(string name);
    }
}
